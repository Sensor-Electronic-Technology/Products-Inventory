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
        private bool _canAdmin=false;
        private bool _canBasic = false;
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("ProductRankLotNotifications"); } }
        public IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("ProductRankLotDispatcher"); } }
        public IDialogService RenameLotDialog { get => ServiceContainer.GetService<IDialogService>("RenameLotDialog"); }
        public IDialogService ReturnPartialDialog { get => ServiceContainer.GetService<IDialogService>("ReturnPartialDialog"); }
        public IDialogService FileNameDialog { get { return ServiceContainer.GetService<IDialogService>("FileNameDialog"); } }
        public IOpenFileDialogService OpenFileDialogService { get { return ServiceContainer.GetService<IOpenFileDialogService>("OpenFileDialog"); } }
        public ISaveFileDialogService SaveFileDialogService { get { return ServiceContainer.GetService<ISaveFileDialogService>("SaveFileDialog"); } }


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
        private ProductTransaction _selectedTransaction = new ProductTransaction();
        private ProductInstance _selectedRankLot = new ProductInstance();
        private ObservableCollection<ProductInstance> _selectedRanks = new ObservableCollection<ProductInstance>();
        private List<Lot> _lots = new List<Lot>();
        private List<ProductInstance> _ranks = new List<ProductInstance>();
        private List<ProductInstance> _outGoingRanks = new List<ProductInstance>();
        private List<ProductType> _productTypes = new List<ProductType>();
        private List<ProductTransaction> _transactions = new List<ProductTransaction>();
        private ObservableCollection<Warehouse> _warehouses = new ObservableCollection<Warehouse>();
        private ObservableCollection<ProductCostRow> _productCostSummary = new ObservableCollection<ProductCostRow>();
        private ObservableCollection<ProductReservation> _reservations = new ObservableCollection<ProductReservation>();
        private ObservableCollection<Attachment> _attachments = new ObservableCollection<Attachment>();
        private Attachment _selectedAttachment = new Attachment();


        private int _selectedTabIndex = 0;
        private bool outGoingInProgress = false;
        private bool isNewProduct = false;
        private bool isEditProduct = false;
        private bool editInProgress = false;
        private Visibility _visibility;
        private FileNameViewModel _fileNameViewModel = null;

        public PrismCommands.DelegateCommand CheckInCommand { get; private set; }
        public PrismCommands.DelegateCommand CheckOutCommand { get; private set; }
        public PrismCommands.DelegateCommand SaveProductCommand { get; private set; }
        public PrismCommands.DelegateCommand CancelProductCommand { get; private set; }
        public PrismCommands.DelegateCommand RenameLotCommand { get; set; }
        public PrismCommands.DelegateCommand ViewRankCommand { get; set; }
        public PrismCommands.DelegateCommand ViewRankInTableFromLotsCommand { get; set; }

        public PrismCommands.DelegateCommand NewAttachmentCommand { get; private set; }
        public PrismCommands.DelegateCommand DeleteAttachmentCommand { get; private set; }
        public PrismCommands.DelegateCommand DownloadFileCommand { get; private set; }
        public PrismCommands.DelegateCommand OpenFileCommand { get; private set; }

        public AsyncCommand<ExportFormat> ExportTransactionsCommand { get; private set; }
        public AsyncCommand<ExportFormat> ExportLotsCommand { get; private set; }
        public AsyncCommand<ExportFormat> ExportRanksCommand { get; private set; }
        public AsyncCommand<ExportFormat> ExportCostSummaryCommand { get; private set; }

        public AsyncCommand UndoTransactionCommand { get; set; }
        public AsyncCommand AddMultipleToOutgoingCommand { get; private set; }
        public PrismCommands.DelegateCommand ReturnPartialCommand { get; set; }

        public ICommand EditRankCommand { get; private set; }
        public ICommand EditLotCommand { get; private set; }

        public ICommand ReserveStockCommand { get; private set; }
        public ICommand ViewReservationDetailsCommand { get; private set; }
        public ICommand EditReservationCommand { get; private set; }
        public ICommand DeleteReservationCommand { get; private set; }
        public ICommand ViewRankDetailsCommand { get; private set; }
        public ICommand ViewLotDetailsCommand { get; private set; }
        public ICommand AddToOutgoingCommand { get; private set; }

        public ProductsLotRankViewModel(ProductDataManager dataManager,IEventAggregator eventAggregator,IRegionManager regionManager) {
            this._dataManager = dataManager;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            this.ViewRankDetailsCommand = new PrismCommands.DelegateCommand(this.ViewRankDetailHandler,this.CanExecute);
            this.ViewLotDetailsCommand = new PrismCommands.DelegateCommand(this.ViewLotDetailHandler,this.CanExecute);
            this.AddToOutgoingCommand = new PrismCommands.DelegateCommand(this.AddToOutgoingHandler,this.CanExecuteOutgoing);
            this.CheckOutCommand = new PrismCommands.DelegateCommand(this.CheckOutHandler,this.CanExecuteBasic);
            this.SaveProductCommand = new PrismCommands.DelegateCommand(this.SaveProductChangesHandle, this.CanExecuteNewProduct);
            this.CancelProductCommand = new PrismCommands.DelegateCommand(this.DiscardChangesHandler);

            this.ExportCostSummaryCommand = new AsyncCommand<ExportFormat>(this.ExportCostSummaryHandler);
            this.ExportLotsCommand = new AsyncCommand<ExportFormat>(this.ExportLotsHandler);
            this.ExportRanksCommand = new AsyncCommand<ExportFormat>(this.ExportRanksHandler);
            this.ExportTransactionsCommand = new AsyncCommand<ExportFormat>(this.ExportTransactionsHandler);
            this.UndoTransactionCommand = new AsyncCommand(this.UndoTransactionHandler, this.CanExecuteAdmin);

            this.EditLotCommand = new PrismCommands.DelegateCommand(this.EditLotHandler, this.CanExecuteBasic);
            this.EditRankCommand = new PrismCommands.DelegateCommand(this.EditRankHandler, this.CanExecuteBasic);
            this.ReserveStockCommand = new PrismCommands.DelegateCommand(this.ReserveStockHandler, this.CanExecuteBasic);
            this.DeleteReservationCommand = new PrismCommands.DelegateCommand(this.DeleteReservationHandler, this.CanExecuteBasic);
            this.EditReservationCommand = new PrismCommands.DelegateCommand(this.EditReservationHandler, this.CanExecuteBasic);
            this.ViewReservationDetailsCommand = new PrismCommands.DelegateCommand(this.ViewReservationDetailsHandler, this.CanExecute);
            this.RenameLotCommand = new PrismCommands.DelegateCommand(this.RenameLotHandler,this.CanExecuteAdmin);
            this.ReturnPartialCommand = new PrismCommands.DelegateCommand(this.ReturnPartialHandler, this.CanExecuteAdmin);
            this.ViewRankCommand = new PrismCommands.DelegateCommand(this.ViewRankHandler);
            this.ViewRankInTableFromLotsCommand = new PrismCommands.DelegateCommand(this.ViewRankInTableFromLotsHandler);

            this.NewAttachmentCommand = new PrismCommands.DelegateCommand(this.NewAttachmentHandler);
            this.DeleteAttachmentCommand = new PrismCommands.DelegateCommand(this.DeleteAttachmentHandler);
            this.OpenFileCommand = new PrismCommands.DelegateCommand(this.OpenFileHandler);
            this.DownloadFileCommand=new PrismCommands.DelegateCommand(this.DownloadFileHandler);

            this.AddMultipleToOutgoingCommand = new AsyncCommand(this.AddMultipleOutgoingHandler);

            this._eventAggregator.GetEvent<StartOutgoingListEvent>().Subscribe(() => this.outGoingInProgress = true);
            this._eventAggregator.GetEvent<DoneOutgoingListEvent>().Subscribe(this.OutgoingListDoneHandler);
            this._eventAggregator.GetEvent<CancelOutgoingListEvent>().Subscribe(this.OutgoingListDoneHandler);
            this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>().Subscribe(this.LotRankEditingDoneHandler);

            this._canAdmin = this._dataManager.ValidateUser(UserAction.Remove);
            this._canBasic = this._dataManager.ValidateUser(UserAction.CheckIn);

            this.Filter = Constants.FileFilters;
            this.FilterIndex = 12;
            this.Title = "Save File As";
            this.DefaultExt = "*.*";
            this.OverwritePrompt = true;
        }

        public override bool KeepAlive {
            get => false;
        }

        public string Filter { get; set; }
        public int FilterIndex { get; set; }
        public string Title { get; set; }
        public bool DialogResult { get; protected set; }
        public string ResultFileName { get; protected set; }
        public string FileBody { get; set; }
        public string DefaultExt { get; set; }
        public string DefaultFileName { get; set; }
        public bool OverwritePrompt { get; set; }

        public ProductInstance SelectedRank {
            get => this._selectedRank;
            set => SetProperty(ref this._selectedRank, value, "SelectedRank");
        }

        public Attachment SelectedAttachment { 
            get => this._selectedAttachment; 
            set => SetProperty(ref this._selectedAttachment,value, "SelectedAttachment");
        }

        public ProductReservation SelectedReservation {
            get => this._selectedReservation;
            set => SetProperty(ref this._selectedReservation, value);
        }

        public ProductInstance SelectedRankLot {
            get => this._selectedRankLot;
            set => SetProperty(ref this._selectedRankLot, value, "SelectedRankLot");
        }

        public Lot SelectedLot {
            get => this._selectedLot;
            set => SetProperty(ref this._selectedLot, value, "SelectedLot");
        }

        public Product SelectedProduct {
            get => this._selectedProduct;
            set => SetProperty(ref this._selectedProduct, value, "SelectedProduct");
        }

        public ProductTransaction SelectedTransaction {
            get => this._selectedTransaction;
            set => SetProperty(ref this._selectedTransaction, value, "SelectedTransaction");
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

        public ObservableCollection<Attachment> Attachments { 
            get => this._attachments; 
            set => SetProperty(ref this._attachments,value,"Attachments");
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

        public ObservableCollection<ProductInstance> SelectedRanks { 
            get => this._selectedRanks; 
            set => SetProperty(ref this._selectedRanks,value); 
        }

        private void OutgoingListDoneHandler() {
            this.outGoingInProgress = false;
        }

        private async Task AddMultipleOutgoingHandler() {
            if (this.SelectedRanks.Count > 1) {
                if (!this.outGoingInProgress) {
                    await this.DispatcherService.BeginInvoke(() => {
                        NavigationParameters parameters = new NavigationParameters();
                        parameters.Add("Rank", this.SelectedRank);
                        this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.OutgoingProductListView, parameters);
                        this.outGoingInProgress = true;
                        foreach (var rank in this.SelectedRanks) {
                            this._eventAggregator.GetEvent<AddToOutgoingEvent>().Publish(rank);
                        }
                    });
                } else {
                    await this.DispatcherService.BeginInvoke(() => {
                        foreach (var rank in this.SelectedRanks) {
                            this._eventAggregator.GetEvent<AddToOutgoingEvent>().Publish(rank);
                        }
                    });
                }
            }
        }

        private void AddToOutgoingHandler() {
            if (this.SelectedRank != null) {
                if (!this.outGoingInProgress) {
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
                var result=this.RenameLotDialog.ShowDialog(
                    dialogCommands: new[] { saveLotChanges, cancelCommand },
                    title: "Rename Lot",
                    viewModel: vm
                );
                if (result == saveLotChanges) {
                    if(this._selectedLot.LotNumber!=vm.NewLotNumber ^ this._selectedLot.SupplierPoNumber!= vm.NewSupplierPo) {
                        var responce=this._dataManager.RenameLot(this._selectedLot, vm.NewLotNumber, vm.NewSupplierPo);
                        this.MessageBoxService.ShowMessage(responce.Message);
                    } else {
                        this.MessageBoxService.ShowMessage("Error: LotNumber and SupplierPo are the same. ");
                    }

                }
                this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>().Publish();
            }
        }
        
        private async Task UndoTransactionHandler() {
            if (this.SelectedTransaction != null) {
                var responce=this.MessageBoxService.ShowMessage("WARNING: This cannot easily be undone. Continue? ", "Warning", MessageButton.YesNo, MessageIcon.Asterisk);
                if (responce == MessageResult.Yes) {
                    this._eventAggregator.GetEvent<LotRankReservationEditingStartedEvent>().Publish();
                    var result = await this._dataManager.DeleteTransactionAsync(this._selectedTransaction);
                    if (result.Success) {
                        DispatcherService.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage(result.Message, "Info", MessageButton.OK, MessageIcon.Information);
                        });
                    } else {
                        DispatcherService.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage(result.Message, "Error", MessageButton.OK, MessageIcon.Error);
                        });
                    }
                    this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>().Publish();
                }
            }
        }

        private void ReturnPartialHandler() {
            if (this._selectedRank != null) {
                this._eventAggregator.GetEvent<LotRankReservationEditingStartedEvent>().Publish();

                ReturnPartialViewModel vm = new ReturnPartialViewModel();
                vm.SelectedRank = this.SelectedRank;
                UICommand saveChanges = new UICommand() {
                    Caption = "Save Changes",
                    IsDefault = true,
                    Command = new DelegateCommand(() => { }, () => (vm.NewQuantity!=0))
                };
                UICommand cancelCommand = new UICommand() {
                    Caption = "Cancel",
                    IsCancel = true,
                };
                var result = this.ReturnPartialDialog.ShowDialog(
                    dialogCommands: new[] { saveChanges, cancelCommand },
                    title: "Return Quantity To Inventory",
                    viewModel: vm
                );
                if (result == saveChanges) {
                    var responce = this._dataManager.ReturnQuantityToInventory(this._selectedRank, vm.NewQuantity, vm.BuyerPo, vm.RMA);
                    if (responce.Success) {
                        this.MessageBoxService.ShowMessage(responce.Message, "Info", MessageButton.OK, MessageIcon.Information);
                    } else {
                        this.MessageBoxService.ShowMessage(responce.Message, "Error", MessageButton.OK, MessageIcon.Error);
                    }
                }
                this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>().Publish();

            }
        }

        private void ViewRankHandler() {
            if (this.SelectedTransaction != null) {
                this.SelectedRank = this.Ranks.FirstOrDefault(e => e.Id == this._selectedTransaction.InstanceId);
                this.SelectedTabIndex = 1;
            }
        }

        private void ViewRankInTableFromLotsHandler() {
            if (this._selectedLot != null) {
                this.SelectedRank = this.Ranks.FirstOrDefault(e => e.LotNumber==this._selectedLot.LotNumber && e.SupplierPoNumber==this._selectedLot.SupplierPoNumber);
                this.SelectedTabIndex = 1;
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

        private void NewAttachmentHandler() {
            this.OpenFileDialogService.Filter = Constants.FileFilters;
            this.OpenFileDialogService.FilterIndex = this.FilterIndex;
            this.OpenFileDialogService.Title = "Select File To Uplaod";
            var resp = this.OpenFileDialogService.ShowDialog();
            if (resp) {
                var file = this.OpenFileDialogService.File;
                string ext = Path.GetExtension(file.GetFullName());
                string tempFileName = file.Name.Substring(0, file.Name.IndexOf("."));
                //this.MessageBoxService.ShowMessage(file.Name);
                if (File.Exists(file.GetFullName())) {
                    if (this.ShowAttachmentDialog(tempFileName)) {
                        if (this._fileNameViewModel != null) {
                            string dest = Path.Combine(Constants.DestinationDirectory, this._fileNameViewModel.FileName + ext);
                            if (!File.Exists(dest)) {
                                bool success = true;
                                try {
                                    File.Copy(file.GetFullName(), dest);
                                } catch {
                                    this.MessageBoxService.ShowMessage("Copy File Error");
                                    success = false;
                                }
                                if (success) {
                                    Attachment attachment = new Attachment(DateTime.Now, this._fileNameViewModel.FileName, "");
                                    attachment.Description = this._fileNameViewModel.Description;
                                    attachment.InventoryItemId = this.SelectedProduct.Id;
                                    attachment.FileReference = dest;
                                    attachment.Extension = ext;
                                    var temp = this._dataManager.UploadProductAttachment(attachment);
                                    if (temp.Success) {
                                        this.MessageBoxService.ShowMessage(temp.Message, "Success", MessageButton.OK, MessageIcon.Information);
                                    } else {
                                        this.MessageBoxService.ShowMessage(temp.Message, "Failed", MessageButton.OK, MessageIcon.Error);
                                    }
                                }
                            } else {
                                this.MessageBoxService.ShowMessage("File Name already exist, Please try again", "Failed", MessageButton.OK, MessageIcon.Error);
                            }
                        }
                    }
                } else {
                    this.MessageBoxService.ShowMessage("Internal Error: File Not Found", "File Not Found", MessageButton.OK, MessageIcon.Error);
                }
                this.ReloadAsync();
            }            
        }

        private void DeleteAttachmentHandler() {
            if (this.SelectedAttachment != null) {
                string message = "You are about to delete attachment:" + this.SelectedAttachment.Name +
                    Environment.NewLine + "Continue?";
                var result = this.MessageBoxService.ShowMessage(message, "Delete", MessageButton.YesNo, MessageIcon.Asterisk);
                if (result == MessageResult.Yes) {

                    if (File.Exists(this.SelectedAttachment.FileReference)) {
                        var success = true;
                        try {
                            File.Delete(this.SelectedAttachment.FileReference);
                        } catch {
                            this.MessageBoxService.ShowMessage("Failed to Delete Attachment", "Error",MessageButton.OK,MessageIcon.Error);
                            success = false;
                        }
                        if (success) {
                            var responce=this._dataManager.DeleteProductAttachment(this.SelectedAttachment);
                            if (responce.Success) {
                                this.MessageBoxService.ShowMessage(responce.Message, "Success", MessageButton.OK, MessageIcon.Information);
                            } else {
                                this.MessageBoxService.ShowMessage("", "Error", MessageButton.OK, MessageIcon.Error);
                            }
                        }
                    }
                    this.ReloadAsync();
                }
            }
        }

        private void OpenFileHandler() {
            if (this.SelectedAttachment != null) {
                if (File.Exists(this.SelectedAttachment.FileReference)) {
                    Process.Start(this.SelectedAttachment.FileReference);
                }
            }
        }

        private void DownloadFileHandler() {
            if (this.SelectedAttachment != null) {
                if (File.Exists(this.SelectedAttachment.FileReference)) {
                    string file = this.SelectedAttachment.FileReference;
                    string ext = Path.GetExtension(file);
                    this.SaveFileDialogService.DefaultExt = ext;
                    this.SaveFileDialogService.DefaultFileName = Path.GetFileName(file);
                    this.SaveFileDialogService.Filter = this.Filter;
                    this.SaveFileDialogService.FilterIndex = this.FilterIndex;
                    this.DialogResult = SaveFileDialogService.ShowDialog();
                    if (this.DialogResult) {
                        File.Copy(file, this.SaveFileDialogService.File.GetFullName());
                    }
                } else {
                    this.MessageBoxService.ShowMessage("File doesn't exist??");
                }
            } else {
                this.MessageBoxService.ShowMessage("Selection is null??");
            }
        }

        private bool ShowAttachmentDialog(string currentFile) {
            if (this._fileNameViewModel == null) {
                this._fileNameViewModel = new FileNameViewModel(currentFile);
            }
            this._fileNameViewModel.FileName = currentFile;
            this._fileNameViewModel.Description = "";

            UICommand saveCommand = new UICommand() {
                Caption = "Save Attachment",
                IsCancel = false,
                IsDefault = true,
            };

            UICommand cancelCommand = new UICommand() {
                Id = MessageBoxResult.Cancel,
                Caption = "Cancel",
                IsCancel = true,
                IsDefault = false,
            };
            UICommand result = FileNameDialog.ShowDialog(
            dialogCommands: new List<UICommand>() { saveCommand, cancelCommand },
            title: "New Attachment Dialog",
            viewModel: this._fileNameViewModel);
            return result == saveCommand;
        }

        private async void ReloadAsync() {

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

            var attachments=(await this._dataManager.AttachmentProvider.GetEntityListAsync()).ToList();
            ObservableCollection<Attachment> temp = new ObservableCollection<Attachment>();
            temp.AddRange(attachments);
            this.Attachments = temp;

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

            var attachments = this._dataManager.AttachmentProvider.GetEntityList().ToList();
            ObservableCollection<Attachment> tempAttach = new ObservableCollection<Attachment>();
            tempAttach.AddRange(attachments);
            this.Attachments = tempAttach;

        }

        private bool CanExecute() {
            return !this.outGoingInProgress && !this.isNewProduct && !this.editInProgress && !this.isEditProduct;
        }

        private bool CanExecuteBasic() {
            return !this.outGoingInProgress && !this.isNewProduct && !this.editInProgress && !this.isEditProduct && this._canBasic;
        }

        private bool CanExecuteAdmin() {
            return !this.outGoingInProgress && !this.isNewProduct && !this.editInProgress && !this.isEditProduct && this._canAdmin;
        }

        private bool CanExecuteOutgoing() {
            return !this.isNewProduct && !this.editInProgress && !this.isEditProduct  && this._canBasic;
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
                this.Load();
            }
        }
    }
}