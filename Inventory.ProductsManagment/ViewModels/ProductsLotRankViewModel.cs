using System;
using System.Collections.Generic;
using System.Linq;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.Common.ApplicationLayer;
using DevExpress.Mvvm;
using Prism.Regions;
using Prism.Events;
using PrismCommands = Prism.Commands;
using System.Threading.Tasks;
using Inventory.Common.DataLayer.EntityDataManagers;
using System.Windows.Input;
using System.Windows;
using Inventory.Common.ApplicationLayer.Services;
using System.Collections.ObjectModel;
using Inventory.Common.BuisnessLayer;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Inventory.ProductsManagment.ViewModels {
    public class ProductsLotRankViewModel : InventoryViewModelNavigationBase {
        static object SyncRoot = new object();
        private ProductDataManager _dataManager;
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("ProductRankLotNotifications"); } }
        public IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("ProductRankLotDispatcher"); } }
        public IDialogService DialogService { get => ServiceContainer.GetService<IDialogService>("RenameLotDialog"); }

        public IExportService CostSummaryExportServive { get => ServiceContainer.GetService<IExportService>("CostSummaryExportService"); }
        public IExportService RankExportService { get => ServiceContainer.GetService<IExportService>("RankExportService"); }
        public IExportService LotExportService { get => ServiceContainer.GetService<IExportService>("LotExportService"); }
        public IExportService TransactionExportService { get => ServiceContainer.GetService<IExportService>("TransactionExportService"); }

        public IControlUpdateService GridUpdaterService { get { return ServiceContainer.GetService<IGridUpdateService>(); } }

        private ProductInstance _selectedRank = new ProductInstance();
        private Lot _selectedLot = new Lot();
        private Product _selectedProduct = new Product();
        private ProductType _selectedProductType = new ProductType();
        private Warehouse _selectedWarehouse = new Warehouse();
        private ProductReservation _selectedReservation = new ProductReservation();
        private List<Lot> _lots = new List<Lot>();
        private List<ProductInstance> _ranks = new List<ProductInstance>();
        private List<ProductInstance> _outGoingRanks = new List<ProductInstance>();
        private List<ProductType> _productTypes = new List<ProductType>();
        private List<ProductTransaction> _transactions = new List<ProductTransaction>();
        private ObservableCollection<Warehouse> _warehouses = new ObservableCollection<Warehouse>();
        private ObservableCollection<ProductCostRow> _productCostSummary = new ObservableCollection<ProductCostRow>();
        private ObservableCollection<ProductReservation> _reservations = new ObservableCollection<ProductReservation>();

        private int _selectedTabIndex = 0;
        private bool outGoingInProgress = false;
        private bool isNewProduct = false;
        private bool isEditProduct = false;
        private bool editInProgress = false;
        private Visibility _visibility;

        public PrismCommands.DelegateCommand CheckInCommand { get; private set; }
        public PrismCommands.DelegateCommand CheckOutCommand { get; private set; }
        public PrismCommands.DelegateCommand SaveProductCommand { get; private set; }
        public PrismCommands.DelegateCommand CancelProductCommand { get; private set; }
        public PrismCommands.DelegateCommand RenameLotCommand { get; set; }

        public AsyncCommand<ExportFormat> ExportTransactionsCommand { get; private set; }
        public AsyncCommand<ExportFormat> ExportLotsCommand { get; private set; }
        public AsyncCommand<ExportFormat> ExportRanksCommand { get; private set; }
        public AsyncCommand<ExportFormat> ExportCostSummaryCommand { get; private set; }


        public ICommand EditRankCommand { get; private set; }
        public ICommand EditLotCommand { get; private set; }

        public ICommand ReserveStockCommand { get; private set; }
        public ICommand ViewReservationDetailsCommand { get; private set; }
        public ICommand EditReservationCommand { get; private set; }
        public ICommand DeleteReservationCommand { get; private set; }
        public ICommand ViewRankDetailsCommand { get; private set; }
        public ICommand ViewLotDetailsCommand { get; private set; }
        public ICommand AddToOutgoingCommand { get; private set; }

        //Attachment Commands
        public PrismCommands.DelegateCommand NewAttachmentCommand { get; private set; }
        public PrismCommands.DelegateCommand<object> DeleteAttachmentCommand { get; private set; }
        public PrismCommands.DelegateCommand<object> DownloadFileCommand { get; private set; }
        public PrismCommands.DelegateCommand<object> OpenFileCommand { get; private set; }

        //Exports



        public ProductsLotRankViewModel(ProductDataManager dataManager,IEventAggregator eventAggregator,IRegionManager regionManager) {
            this._dataManager = dataManager;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            this.ViewRankDetailsCommand = new PrismCommands.DelegateCommand(this.ViewRankDetailHandler,this.CanExecute);
            this.ViewLotDetailsCommand = new PrismCommands.DelegateCommand(this.ViewLotDetailHandler,this.CanExecute);
            this.AddToOutgoingCommand = new PrismCommands.DelegateCommand(this.AddToOutgoingHandler,this.CanExecuteOutgoing);
            this.CheckOutCommand = new PrismCommands.DelegateCommand(this.CheckOutHandler,this.CanExecute);
            this.SaveProductCommand = new PrismCommands.DelegateCommand(this.SaveProductChangesHandle, this.CanExecuteNewProduct);
            this.CancelProductCommand = new PrismCommands.DelegateCommand(this.DiscardChangesHandler);

            this.ExportCostSummaryCommand = new AsyncCommand<ExportFormat>(this.ExportCostSummaryHandler);
            this.ExportLotsCommand = new AsyncCommand<ExportFormat>(this.ExportLotsHandler);
            this.ExportRanksCommand = new AsyncCommand<ExportFormat>(this.ExportRanksHandler);
            this.ExportTransactionsCommand = new AsyncCommand<ExportFormat>(this.ExportTransactionsHandler);

            this.EditLotCommand = new PrismCommands.DelegateCommand(this.EditLotHandler, this.CanExecute);
            this.EditRankCommand = new PrismCommands.DelegateCommand(this.EditRankHandler, this.CanExecute);
            this.ReserveStockCommand = new PrismCommands.DelegateCommand(this.ReserveStockHandler, this.CanExecute);
            this.DeleteReservationCommand = new PrismCommands.DelegateCommand(this.DeleteReservationHandler, this.CanExecute);
            this.EditReservationCommand = new PrismCommands.DelegateCommand(this.EditReservationHandler, this.CanExecute);
            this.ViewReservationDetailsCommand = new PrismCommands.DelegateCommand(this.ViewReservationDetailsHandler, this.CanExecute);
            this.RenameLotCommand = new PrismCommands.DelegateCommand(this.RenameLotHandler);

            this._eventAggregator.GetEvent<StartOutgoingListEvent>().Subscribe(() => this.outGoingInProgress = true);
            this._eventAggregator.GetEvent<DoneOutgoingListEvent>().Subscribe(this.OutgoingListDoneHandler);
            this._eventAggregator.GetEvent<CancelOutgoingListEvent>().Subscribe(this.OutgoingListDoneHandler);
            this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>().Subscribe(this.LotRankEditingDoneHandler);
        }

        public override bool KeepAlive {
            get => false;
        }

        public ProductInstance SelectedRank {
            get => this._selectedRank;
            set => SetProperty(ref this._selectedRank, value, "SelectedRank");
        }

        public ProductReservation SelectedReservation {
            get => this._selectedReservation;
            set => SetProperty(ref this._selectedReservation, value);
        }

        public Lot SelectedLot {
            get => this._selectedLot;
            set => SetProperty(ref this._selectedLot, value, "SelectedLot");
        }

        public Product SelectedProduct {
            get => this._selectedProduct;
            set => SetProperty(ref this._selectedProduct, value, "SelectedProduct");
        }

        public ProductType SelectedProductType {
            get => this._selectedProductType;
            set => SetProperty(ref this._selectedProductType, value, "SelectedProductType");
        }

        public Warehouse SelectedWareHouse {
            get => this._selectedWarehouse;
            set => SetProperty(ref this._selectedWarehouse, value,"SelectedWarehouse");
        }

        public int SelectedTabIndex {
            get => this._selectedTabIndex;
            set => SetProperty(ref this._selectedTabIndex,value,"SelectedTabIndex");
        }

        public List<ProductInstance> Ranks {
            get => this._ranks;
            set => SetProperty(ref this._ranks, value, "Ranks");
        }

        public List<Lot> Lots {
            get => this._lots;
            set => SetProperty(ref this._lots, value, "Lots");
        }

        public List<ProductType> ProductTypes {
            get => this._productTypes;
            set => SetProperty(ref this._productTypes, value, "ProductTypes");
        }

        public ObservableCollection<Warehouse> Warehouses {
            get => this._warehouses;
            set => SetProperty(ref this._warehouses, value,"Warehouses");
        }

        public List<ProductTransaction> Transactions {
            get => this._transactions;
            set => SetProperty(ref this._transactions, value, "Transactions");
        }

        public ObservableCollection<ProductCostRow> ProductCostSummary {
            get => this._productCostSummary;
            set => SetProperty(ref this._productCostSummary,value);
        }

        public ObservableCollection<ProductReservation> Reservations {
            get => this._reservations;
            set => SetProperty(ref this._reservations, value);
        }

        public Visibility Visibility {
            get => this._visibility;
            set => SetProperty(ref this._visibility, value, "Visibility");
        }

        private void OutgoingListDoneHandler() {
            this.outGoingInProgress = false;
        }

        private void AddToOutgoingHandler() {
            if(this.SelectedRank != null) {
                if(!this.outGoingInProgress) {
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add("Rank", this.SelectedRank);
                    this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.OutgoingProductListView, parameters);
                } else {
                    this._eventAggregator.GetEvent<AddToOutgoingEvent>().Publish(this.SelectedRank);
                }
            }
        }

        private void ViewRankDetailHandler() {
            if(this.SelectedRank != null) {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Rank", this.SelectedRank);
                parameters.Add("IsEdit", false);
                this.editInProgress = false;
                this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.RankDetailsView, parameters);
            }
        }

        private void EditRankHandler() {
            if(this.SelectedRank != null) {
                if(this.SelectedRank.Transactions.Count != 0) {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat("Warning: Rank: {0} has Transactions associated with it", this.SelectedRank.Name)
                        .AppendLine("You will not be able to edit the Quantity but all other fields will be available for editing")
                        .AppendLine()
                        .AppendLine("If You Need to Change Quantity Please Cancel and Perform a Corrective Tranasaction")
                        .AppendLine()
                        .AppendLine("Press Okay To Continue");
                    var responce=this.MessageBoxService.ShowMessage(builder.ToString(),"Warning",MessageButton.OKCancel,MessageIcon.Warning,MessageResult.Cancel);
                    if(responce == MessageResult.OK) {
                        this._eventAggregator.GetEvent<LotRankReservationEditingStartedEvent>().Publish();
                        NavigationParameters parameters = new NavigationParameters();
                        parameters.Add("Rank", this.SelectedRank);
                        parameters.Add("IsEdit", true);
                        parameters.Add("CanEditQuantity", false);
                        this.editInProgress = true;
                        this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.RankDetailsView, parameters);
                    }
                } else {
                    this._eventAggregator.GetEvent<LotRankReservationEditingStartedEvent>().Publish();
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add("Rank", this.SelectedRank);
                    parameters.Add("IsEdit", true);
                    parameters.Add("CanEditQuantity", true);
                    this.editInProgress = true;
                    this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.RankDetailsView, parameters);
                }

            }
        }

        private void ReserveStockHandler() {
            if(this.SelectedRank != null) {
                this._eventAggregator.GetEvent<LotRankReservationEditingStartedEvent>().Publish();
                ProductReservation reservation = new ProductReservation(this.SelectedRank);
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Reservation", reservation);
                parameters.Add("IsEdit", false);
                parameters.Add("IsNew", true);
                this.editInProgress = true;
                this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.ProductReservationView, parameters);
            }
        }

        private void ViewReservationDetailsHandler() {
            if(this.SelectedReservation != null) {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Reservation", this.SelectedReservation);
                parameters.Add("IsEdit", false);
                parameters.Add("IsNew", false);
                this.editInProgress = false;
                this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.ProductReservationView, parameters);
            }
        }

        private void EditReservationHandler() {
            if(this.SelectedReservation != null) {
                this._eventAggregator.GetEvent<LotRankReservationEditingStartedEvent>().Publish();
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Reservation", this.SelectedReservation);
                parameters.Add("IsEdit", true);
                parameters.Add("IsNew", false);
                this.editInProgress = true;
                this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.ProductReservationView, parameters);
            }
        }

        private void DeleteReservationHandler() {
            if(this.SelectedReservation != null) {
                var responce = this.MessageBoxService.ShowMessage("You are about to delete Product Reservation"
                    + Environment.NewLine
                    + "This cannot be undone, Continue?"
                    , "Warning", MessageButton.YesNo, MessageIcon.Warning, MessageResult.No);
                if(responce == MessageResult.Yes) {
                    var deleted = this._dataManager.ReservationOperations.Delete(this.SelectedReservation);
                    if(deleted != null) {
                        this.DispatcherService.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Reservation Succesfully Deleted", "Success", MessageButton.OK, MessageIcon.Information);
                        });
                        this.ReloadAsync();
                    } else {
                        this.DispatcherService.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Delete Failed, No Changes Made","Error",MessageButton.OK,MessageIcon.Error);
                        });
                    }
                } else {
                    this.DispatcherService.BeginInvoke(() => {
                        this.MessageBoxService.ShowMessage("Delete Canceled, No Changes Made", "Error", MessageButton.OK, MessageIcon.Error);
                    });
                }
            }
        }

        private void ViewLotDetailHandler() {
            if(this.SelectedLot != null) {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Lot", this.SelectedLot);
                parameters.Add("IsEdit", false);
                this.editInProgress = false;
                this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.LotDetailsView, parameters);
            }
        }

        private void EditLotHandler() {
            if(this.SelectedLot != null) {
                this._eventAggregator.GetEvent<LotRankReservationEditingStartedEvent>().Publish();
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Lot", this.SelectedLot);
                parameters.Add("IsEdit", true);
                this.editInProgress = true;
                this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.LotDetailsView, parameters);
            }
        }

        private void LotRankEditingDoneHandler() {
            this.editInProgress = false;
            this.ReloadAsync();
        }

        private void CheckOutHandler() {
            if(this.SelectedRank != null) {
                if(!this.outGoingInProgress) {
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add("Rank", this.SelectedRank);
                    this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.OutgoingProductListView, parameters);
                } else {
                    this._eventAggregator.GetEvent<AddToOutgoingEvent>().Publish(this.SelectedRank);
                }
            } else {
                this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.OutgoingProductListView);
            }
        }

        private void SaveProductChangesHandle() {
            if(this.SelectedProductType != null && this.SelectedWareHouse!=null) {
                this.SelectedProduct.ProductTypeId = this.SelectedProductType.Id;
                this.SelectedProduct.WarehouseId = this.SelectedWareHouse.Id;
                Product entity;
                if(this.isEditProduct) {
                    entity = this._dataManager.ProductOperations.Update(this.SelectedProduct);
                } else if(this.isNewProduct) {
                    entity = this._dataManager.ProductOperations.Add(this.SelectedProduct);
                } else {
                    entity = null;
                }

                if(entity != null) {
                    this.Visibility = Visibility.Collapsed;
                    this._eventAggregator.GetEvent<ProductEditingDoneEvent>().Publish();
                    DispatcherService.BeginInvoke(() => {
                        this.MessageBoxService.ShowMessage("Product: " + entity.Name + " Saved"
                            , "Saved", MessageButton.OK, MessageIcon.Information);
                    });
                } else {
                    DispatcherService.BeginInvoke(() => {
                        this.MessageBoxService.ShowMessage("Error Saving, Please Check Input or Discard Changes", "Error", MessageButton.OK, MessageIcon.Error);
                    });
                }
            } else {
                DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage("No Package Type Selected, " + Environment.NewLine +
                        "Please Check Selection And Try Again.", "Error", MessageButton.OK, MessageIcon.Error);
                });
            }
        }

        private void DiscardChangesHandler() {
            this.editInProgress = false;
            this.isNewProduct = false;
            this.isEditProduct = false;
            this.outGoingInProgress = false;
            this.Visibility = Visibility.Collapsed;
            this._eventAggregator.GetEvent<ProductEditingDoneEvent>().Publish();
            this.ReloadAsync();
        }

        private void RenameLotHandler() {
            if (this.SelectedLot != null) {
                this._eventAggregator.GetEvent<LotRankReservationEditingStartedEvent>().Publish();
                var vm = new RenameLotViewModel() { NewLotNumber = this.SelectedLot.LotNumber, NewSupplierPo = this.SelectedLot.SupplierPoNumber };
                UICommand saveLotChanges = new UICommand() {
                    Caption = "Save Changes",
                    IsDefault = true,
                    Command = new DelegateCommand(() => { }, () => (!string.IsNullOrEmpty(vm.NewLotNumber) && !string.IsNullOrEmpty(vm.NewSupplierPo)))
                };
                UICommand cancelCommand = new UICommand() {
                    Caption = "Cancel",
                    IsCancel = true,
                };
                var result=this.DialogService.ShowDialog(
                    dialogCommands: new[] { saveLotChanges, cancelCommand },
                    title: "Rename Lot",
                    viewModel: vm
                );
                if (result == saveLotChanges) {
                    this.MessageBoxService.ShowMessage("Changes Saved!");
                }
                this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>().Publish();
            }
        }
        
        private async Task ExportTransactionsHandler(ExportFormat format) {
            await Task.Run(() => { 
                this.DispatcherService.BeginInvoke(() => {
                    var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
                    using(FileStream file = File.Create(path)) {
                        this.TransactionExportService.Export(file, format);
                    }
                    Process.Start(path);
                });
            });
        }

        private async Task ExportLotsHandler(ExportFormat format) {
            await Task.Run(() => { 
            this.DispatcherService.BeginInvoke(() => {
                var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
                using(FileStream file = File.Create(path)) {
                    this.LotExportService.Export(file, format, new DevExpress.XtraPrinting.XlsxExportOptionsEx() {
                        ExportType = DevExpress.Export.ExportType.WYSIWYG
                    });
                }
                Process.Start(path);
            });
            });
        }

        private async Task ExportRanksHandler(ExportFormat format) {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
                    using(FileStream file = File.Create(path)) {
                        this.RankExportService.Export(file, format);
                    }
                    Process.Start(path);
                });
            });
        }

        private async Task ExportCostSummaryHandler(ExportFormat format) {
            await Task.Run(() => { 
            this.DispatcherService.BeginInvoke(() => {
                var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
                using(FileStream file = File.Create(path)) {
                    this.CostSummaryExportServive.Export(file, format);
                }
                Process.Start(path);
            });
            });
        }

        private async void ReloadAsync() {
            //await this._dataManager.LotProvider.LoadDataAsync();
            //await this._dataManager.RankProvider.LoadDataAsync();
            //Load Ranks
            await Task.Run(() => {
                this.Ranks = this._dataManager.RankProvider.GetEntityList(x => x.InventoryItemId == this.SelectedProduct.Id).ToList();
            });

            //load Cost Summary/Lots
            await Task.Run(() => {
                this.Lots = this._dataManager.LotProvider.GetEntityList(x => x.ProductId == this.SelectedProduct.Id).ToList();
                ObservableCollection<ProductCostRow> data = new ObservableCollection<ProductCostRow>();
                this.Lots.ForEach(lot => {
                    if(lot.Cost != null) {
                        if(lot.Cost.Amount > 0) {
                            lot.ProductInstances.ToList().ForEach(rank => {
                                if(rank.Quantity > 0) {
                                    data.Add(new ProductCostRow(rank, lot.Cost.Amount));
                                }
                            });
                        }
                    }
                });
                this.ProductCostSummary = data;
            });

            //Load Transactions
            await Task.Run(() => {
                this.Transactions = (from rank in this.Ranks
                                     from transaction in rank.Transactions
                                     select (ProductTransaction)transaction).ToList();
            });

            //Load ProductTypes
            await Task.Run(() => {
                this.ProductTypes = this._dataManager.CategoryProvider.GetEntityList().OrderBy(e => e.Name).ToList();
                if(this.SelectedProduct.ProductType != null) {
                    this.SelectedProductType = this.ProductTypes.FirstOrDefault(x => x.Name == this.SelectedProduct.ProductType.Name);
                }
            });

            //Load Reservations
            await Task.Run(() => {
                var reservations = this._dataManager.ReservationProvider.GetEntityList(e => e.ProductName == this.SelectedProduct.Name);
                ObservableCollection<ProductReservation> temp = new ObservableCollection<ProductReservation>();
                temp.AddRange(reservations);
                this.Reservations = temp;
            });
        }

        private async Task ReloadAsyncTask() {

            //Load Ranks
            await Task.Run(() => {
                this.Ranks = this._dataManager.RankProvider.GetEntityList(x => x.InventoryItemId == this.SelectedProduct.Id).ToList();
            });

            //load Cost Summary/Lots
            await Task.Run(() => {
                this.Lots = this._dataManager.LotProvider.GetEntityList(x => x.ProductId == this.SelectedProduct.Id).ToList();
                ObservableCollection<ProductCostRow> data = new ObservableCollection<ProductCostRow>();
                this.Lots.ForEach(lot => {
                    if(lot.Cost != null) {
                        if(lot.Cost.Amount > 0) {
                            lot.ProductInstances.ToList().ForEach(rank => {
                                if(rank.Quantity > 0) {
                                    data.Add(new ProductCostRow(rank, lot.Cost.Amount));
                                }
                            });
                        }
                    }
                });
                this.ProductCostSummary = data;
            });

            //Load Transactions
            await Task.Run(() => {
                this.Transactions = (from rank in this.Ranks
                                     from transaction in rank.Transactions
                                     select (ProductTransaction)transaction).ToList();
            });

            //Load ProductTypes
            await Task.Run(() => {
                this.ProductTypes = this._dataManager.CategoryProvider.GetEntityList().OrderBy(e => e.Name).ToList();
                if(this.SelectedProduct.ProductType != null) {
                    this.SelectedProductType = this.ProductTypes.FirstOrDefault(x => x.Name == this.SelectedProduct.ProductType.Name);
                }
            });

            await Task.Run(() => {
                var temp= this._dataManager.WarehouseProvider.GetEntityList().OrderBy(e => e.Name).ToList();
                if (this.SelectedProduct.Warehouse != null) {
                    this.SelectedWareHouse = this.Warehouses.FirstOrDefault(x => x.Name == this.SelectedProduct.Warehouse.Name);
                }
            });

            //Load Reservations
            await Task.Run(() => {
                var reservations = this._dataManager.ReservationProvider.GetEntityList(e => e.ProductName == this.SelectedProduct.Name);
                ObservableCollection<ProductReservation> temp = new ObservableCollection<ProductReservation>();
                temp.AddRange(reservations);
                this.Reservations = temp;
            });
        }

        private async void LoadDataAsync() {
            //Load Ranks
            await Task.Run(() => {
                this.Ranks = this._dataManager.RankProvider.GetEntityList(x => x.InventoryItemId == this.SelectedProduct.Id).ToList();
            });

            //load Cost Summary/Lots
            await Task.Run(() => {
                this.Lots = this._dataManager.LotProvider.GetEntityList(x => x.ProductId == this.SelectedProduct.Id).ToList();
                ObservableCollection<ProductCostRow> data = new ObservableCollection<ProductCostRow>();
                this.Lots.ForEach(lot => {
                    if(lot.Cost!=null) {
                        if(lot.Cost.Amount > 0) {
                            lot.ProductInstances.ToList().ForEach(rank => {
                                if(rank.Quantity > 0) {
                                    data.Add(new ProductCostRow(rank, lot.Cost.Amount));
                                }
                            });
                        }
                    }
                });
                this.ProductCostSummary = data;
            });

            //Load Transactions
            await Task.Run(() => {
                this.Transactions = (from rank in this.Ranks
                                     from transaction in rank.Transactions
                                     select (ProductTransaction)transaction).ToList();
            });

            //Load ProductTypes
            await Task.Run(() => {
                this.ProductTypes = this._dataManager.CategoryProvider.GetEntityList().OrderBy(e => e.Name).ToList();
                if(this.SelectedProduct.ProductType != null) {
                    this.SelectedProductType = this.ProductTypes.FirstOrDefault(x => x.Name == this.SelectedProduct.ProductType.Name);
                }
            });
           
            //Load Reservations
            await Task.Run(() => {
                var reservations = this._dataManager.ReservationProvider.GetEntityList(e => e.ProductName == this.SelectedProduct.Name);
                ObservableCollection<ProductReservation> temp = new ObservableCollection<ProductReservation>();
                temp.AddRange(reservations);
                this.Reservations = temp;
            }).ContinueWith(b=> {
                this.Warehouses = new ObservableCollection<Warehouse>(this._dataManager.WarehouseProvider.GetEntityList().OrderBy(e => e.Name).ToList());
                if (this.SelectedProduct.Warehouse != null) {
                    this.SelectedWareHouse = this.Warehouses.FirstOrDefault(x => x.Name == this.SelectedProduct.Warehouse.Name);
                }
            });
        }

        private void Load() {
            //Load Ranks
            this.Ranks = this._dataManager.RankProvider.GetEntityList(x => x.InventoryItemId == this.SelectedProduct.Id).ToList();


            //load Cost Summary/Lots
                this.Lots = this._dataManager.LotProvider.GetEntityList(x => x.ProductId == this.SelectedProduct.Id).ToList();
                ObservableCollection<ProductCostRow> data = new ObservableCollection<ProductCostRow>();
                this.Lots.ForEach(lot => {
                    if (lot.Cost != null) {
                        if (lot.Cost.Amount > 0) {
                            lot.ProductInstances.ToList().ForEach(rank => {
                                if (rank.Quantity > 0) {
                                    data.Add(new ProductCostRow(rank, lot.Cost.Amount));
                                }
                            });
                        }
                    }
                });
                this.ProductCostSummary = data;


            //Load Transactions
                this.Transactions = (from rank in this.Ranks
                                     from transaction in rank.Transactions
                                     select (ProductTransaction)transaction).ToList();


            //Load ProductTypes
                this.ProductTypes = this._dataManager.CategoryProvider.GetEntityList().OrderBy(e => e.Name).ToList();
                if (this.SelectedProduct.ProductType != null) {
                    this.SelectedProductType = this.ProductTypes.FirstOrDefault(x => x.Name == this.SelectedProduct.ProductType.Name);
                }


            //Load Reservations
                var reservations = this._dataManager.ReservationProvider.GetEntityList(e => e.ProductName == this.SelectedProduct.Name);
                ObservableCollection<ProductReservation> temp = new ObservableCollection<ProductReservation>();
                temp.AddRange(reservations);
                this.Reservations = temp;

                this.Warehouses = new ObservableCollection<Warehouse>(this._dataManager.WarehouseProvider.GetEntityList().OrderBy(e => e.Name).ToList());
            if (this.SelectedProduct.Warehouse != null) {
                this.SelectedWareHouse = this.Warehouses.FirstOrDefault(x => x.Name == this.SelectedProduct.Warehouse.Name);
            }

        }

        private bool CanExecute() {
            return !this.outGoingInProgress && !this.isNewProduct && !this.editInProgress && !this.isEditProduct;
        }

        private bool CanExecuteOutgoing() {
            return !this.isNewProduct && !this.editInProgress && !this.isEditProduct;
        }

        private bool CanExecuteNewProduct() {
            return this.SelectedProduct.Name != Constants.DefaultNewProductName;
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            var product = navigationContext.Parameters["Product"] as Product;
            if(product != null) {
                return this.SelectedProduct != null && this.SelectedProduct.Name == product.Name;
            } else {
                return true;
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var product = navigationContext.Parameters["Product"] as Product;
            var isOngoing = Convert.ToBoolean(navigationContext.Parameters["Ongoing"]);
            var isNew = Convert.ToBoolean(navigationContext.Parameters["IsNew"]);
            var isEdit = Convert.ToBoolean(navigationContext.Parameters["IsEdit"]);      
            if(product != null) {
                this.isNewProduct = isNew;
                this.SelectedProduct = product;
                this.outGoingInProgress = isOngoing;
                this.isEditProduct = isEdit;
                this.Visibility = (isNew || isEdit) ? Visibility.Visible : Visibility.Collapsed;
                //this.LoadDataAsync();
                this.Load();
            }
        }
    }
}