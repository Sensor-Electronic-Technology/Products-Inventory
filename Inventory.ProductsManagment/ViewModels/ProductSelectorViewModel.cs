using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.Common.BuisnessLayer;
using Inventory.Common.ApplicationLayer;
using DevExpress.Mvvm;
using Prism.Regions;
using Prism.Events;
using PrismCommands = Prism.Commands;
using System.Threading.Tasks;
using Inventory.Common.DataLayer.EntityDataManagers;
using Inventory.Common.DataLayer.Providers;
using System.Windows.Input;
using System.Windows;
using Inventory.Common.ApplicationLayer.Services;
using System.Threading;

namespace Inventory.ProductsManagment.ViewModels {
    public class ProductSelectorViewModel : InventoryViewModelBase {
        static object SyncRoot = new object();

        private ProductDataManager _dataManager;
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("ProductSelectorNotifications"); } }
        public IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("ProductSelectorDispatcher"); } }
        public IControlUpdateService GridUpdateService { get { return ServiceContainer.GetService<IGridUpdateService>(); } }

        private List<Product> _products = new List<Product>();
        private Product _selectedProduct = new Product();
        private bool outgoingInProgress = false;
        private bool incomingInProgress = false;
        private bool editInProgress = false;

        public PrismCommands.DelegateCommand NewProductCommand { get; private set; }
        public PrismCommands.DelegateCommand StartIncomingWithDelegate { get; private set; }
        public PrismCommands.DelegateCommand ViewProductDetailsDelegate { get; private set; }
        public PrismCommands.DelegateCommand SetInIncomingFormDelegate { get; private set; }
        public PrismCommands.DelegateCommand EditProductDelegate { get; private set; }
        public PrismCommands.DelegateCommand DeleteProductCommand { get; private set; }
        public PrismCommands.DelegateCommand OnCopyingToClipboardCommand { get; private set; }
        public AsyncCommand RefreshDataCommand { get; private set; }

        public ProductSelectorViewModel(ProductDataManager dataManager, IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._dataManager = dataManager;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            this.ViewProductDetailsDelegate = new PrismCommands.DelegateCommand(this.ViewProductDetailsHandler, this.CanExecuteViewDetails);
            this.StartIncomingWithDelegate = new PrismCommands.DelegateCommand(this.StartWithSelectedHandler, this.CanExecute);
            this.NewProductCommand = new PrismCommands.DelegateCommand(this.CreateNewProductHandler, this.CanExecute);
            this.SetInIncomingFormDelegate = new PrismCommands.DelegateCommand(this.SetInIncomingHandler, this.CanExecuteIncoming);
            this.EditProductDelegate = new PrismCommands.DelegateCommand(this.EditProductHandler, this.CanExecute);
            this.RefreshDataCommand = new AsyncCommand(this.RefreshHandler, this.CanExecute);

            this.OnCopyingToClipboardCommand = new PrismCommands.DelegateCommand(this.OnCopyingToClipboardHandler);

            this._eventAggregator.GetEvent<StartOutgoingListEvent>().Subscribe(() => this.outgoingInProgress = true);
            this._eventAggregator.GetEvent<DoneOutgoingListEvent>().Subscribe(this.OutgoingListDone);
            this._eventAggregator.GetEvent<CancelOutgoingListEvent>().Subscribe(this.OutgoingListDone);

            this._eventAggregator.GetEvent<DoneIncomingListEvent>().Subscribe(this.CheckInDoneHandler);
            this._eventAggregator.GetEvent<CancelIncomingListEvent>().Subscribe(this.CheckInDoneHandler);

            this._eventAggregator.GetEvent<ProductEditingDoneEvent>().Subscribe(this.EditingDoneHandler);

            this._eventAggregator.GetEvent<LotRankReservationEditingStartedEvent>().Subscribe(() => { this.editInProgress = true; });
            this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>().Subscribe(this.EditingDoneHandler);

            this.PopulateAsync();
        }

        public List<Product> Products {
            get => this._products;
            set => SetProperty(ref this._products, value, "Products");
        }

        public Product SelectedProduct {
            get => this._selectedProduct;
            set => SetProperty(ref this._selectedProduct, value, "SelectedProduct");
        }

        public override bool KeepAlive {
            get => false;
        }

        public ICommand ViewProductDetailsCommand {
            get => this.ViewProductDetailsDelegate;
        }

        public ICommand StartIncomingWithSelectedCommand {
            get => this.StartIncomingWithDelegate;
        }

        public ICommand SetInIncomingFormCommand {
            get => this.SetInIncomingFormDelegate;
        }

        public ICommand EditProductCommand {
            get => this.EditProductDelegate;
        }

        private void OnCopyingToClipboardHandler() {
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Text, this.SelectedProduct.Name);
        }

        private void CreateNewProductHandler() {
            this.editInProgress = true;
            Product product = new Product();
            product.Name = "New Part Number";
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("Product", product);
            parameters.Add("Ongoing", false);
            parameters.Add("IsNew", true);
            parameters.Add("IsEdit",false);
            this._regionManager.RequestNavigate(Regions.ProductLotRankRegion, AppViews.ProductsLotRankView, parameters);
        }

        private void EditProductHandler() {
            if(this.SelectedProduct != null) {
                this.editInProgress = true;
                this.outgoingInProgress = false;
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Product", this.SelectedProduct);
                parameters.Add("Ongoing", false);
                parameters.Add("IsNew", false);
                parameters.Add("IsEdit", true);
                this._regionManager.RequestNavigate(Regions.ProductLotRankRegion, AppViews.ProductsLotRankView, parameters);
            }
        }

        private void OutgoingListDone() {
            this.outgoingInProgress = false;
            this._regionManager.Regions[Regions.ProductDetailsRegion].RemoveAll();
            this._regionManager.Regions[Regions.ProductLotRankRegion].RemoveAll();
            this.PopulateAsync();
        }

        private void ViewProductDetailsHandler() {
            if(this.SelectedProduct != null) {
                this._regionManager.Regions[Regions.ProductDetailsRegion].RemoveAll();
                this._regionManager.Regions[Regions.ProductLotRankRegion].RemoveAll();
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Product", this.SelectedProduct);
                parameters.Add("Ongoing", false);
                parameters.Add("IsNew", false);
                parameters.Add("IsEdit", false);
                this._regionManager.RequestNavigate(Regions.ProductLotRankRegion, AppViews.ProductsLotRankView, parameters);
            }
        }

        private void StartWithSelectedHandler() {
            if(this.SelectedProduct != null) {
                if(!string.IsNullOrEmpty(this.SelectedProduct.Name)) {
                    this.incomingInProgress = true;
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add("Product", this.SelectedProduct);
                    this._regionManager.RequestNavigate(Regions.ProductLotRankRegion, AppViews.IncomingProductFormView, parameters);
                } else {
                    DispatcherService.BeginInvoke(() => {
                        this.MessageBoxService.ShowMessage("Invalid Selection, Please Try Selecting Again", "Error", MessageButton.OK, MessageIcon.Error);
                    });
                }
            }
        }

        private void CheckInDoneHandler() {
            this.incomingInProgress = false;
            this._regionManager.Regions[Regions.ProductLotRankRegion].RemoveAll();
            this._regionManager.Regions[Regions.ProductDetailsRegion].RemoveAll();
            this.PopulateAsync();
            this.DispatcherService.BeginInvoke(() => {
                this.MessageBoxService.ShowMessage("Check in Done", "Success", MessageButton.OK, MessageIcon.Information);
            });
        }

        private void EditingDoneHandler() {
            this.editInProgress = false;
            this.PopulateAsync();
        }

        private void SetInIncomingHandler() {
            if(this.SelectedProduct != null) {
                this._eventAggregator.GetEvent<SetInIncomingFormEvent>().Publish(this.SelectedProduct);
            } else {
                DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage("Invalid Selection, Please Try Selecting Again", "Error", MessageButton.OK, MessageIcon.Error);
                });
            }
        }

        private bool CanExecuteViewDetails() {
            return !this.incomingInProgress && !this.editInProgress;
        }

        private bool CanExecute() {
            return !this.incomingInProgress && !this.outgoingInProgress && !this.editInProgress;
        }

        private bool CanExecuteIncoming() {
            return !this.outgoingInProgress && !this.editInProgress && this.incomingInProgress;
        }

        private async Task RefreshHandler() {
            await this._dataManager.UpdateProductTotalsAsync();
            await this._dataManager.ProductProvider.LoadDataAsync();
            await Task.Run(() => {
                lock(SyncRoot) {
                    if(this.GridUpdateService != null) {
                        this.GridUpdateService.BeginUpdate();
                        this.Products = this._dataManager.ProductProvider.GetEntityList().ToList();
                        this.GridUpdateService.EndUpdate();
                    }
                }
            });
        }

        private async void PopulateAsync() {
            await this._dataManager.ProductProvider.LoadDataAsync();
            await Task.Run(() => {
                lock(SyncRoot) {
                    if(this.GridUpdateService != null) {
                        this.GridUpdateService.BeginUpdate();
                        this.Products = this._dataManager.ProductProvider.GetEntityList().ToList();
                        this.GridUpdateService.EndUpdate();
                    }
                }
            });
        }
    }
}