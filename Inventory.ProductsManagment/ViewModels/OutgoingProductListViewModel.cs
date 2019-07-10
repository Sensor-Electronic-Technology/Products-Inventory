using System;
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
using System.Text;
using Inventory.Common.DataLayer.Providers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Inventory.ProductsManagment.ViewModels {
    public class OutgoingProductListViewModel : InventoryViewModelNavigationBase {

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private ProductDataManager _dataManager;
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("OutgoingProductListNotifications"); } }
        public IDispatcherService Dispatcher { get => ServiceContainer.GetService<IDispatcherService>("OutgoingListDispatcher"); }

        private ObservableCollection<ProductInstance> _outgoingList = new ObservableCollection<ProductInstance>();
        private ProductInstance _selectedRank = new ProductInstance();
        private List<Consumer> _consumers = new List<Consumer>();
        private List<string[]> rma_buyers = new List<string[]>();
        private Consumer _selectedConsumer=new Consumer();
        private string _buyerPoNumber;
        private string _rmaNumber;

        public PrismCommands.DelegateCommand RemoveFromOutgoingDelegate { get; private set; }

        public PrismCommands.DelegateCommand CheckOutCommand { get; private set; }
        public PrismCommands.DelegateCommand CancelCommand { get; private set; }


        public OutgoingProductListViewModel(ProductDataManager productDataManager, IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this._dataManager = productDataManager;

            this.RemoveFromOutgoingDelegate = new PrismCommands.DelegateCommand(this.RemoveFromOutgoingHandler);
            this.CheckOutCommand = new PrismCommands.DelegateCommand(this.CheckOutHandler);
            this.CancelCommand = new PrismCommands.DelegateCommand(this.CancelHandler);
            this._eventAggregator.GetEvent<AddToOutgoingEvent>().Subscribe(this.AddToOutgoingHandler);

            this.PopulateConsumers();
        }

        public override bool KeepAlive {
            get => false;
        }

        public ProductInstance SelectedRank {
            get => this._selectedRank;
            set => SetProperty(ref this._selectedRank, value, "SelectedRank");
        }

        public ObservableCollection<ProductInstance> OutgoingList {
            get => this._outgoingList;
            set => SetProperty(ref this._outgoingList, value, "OutgoingList");
        }

        public List<Consumer> Consumers {
            get => this._consumers;
            set=>SetProperty(ref this._consumers, value, "Consumers");
        }

        public Consumer SelectedConsumer {
            get => this._selectedConsumer;
            set => SetProperty(ref this._selectedConsumer, value, "SelectedConsumer");
        }

        public string BuyerPoNumber {
            get => this._buyerPoNumber;
            set => SetProperty(ref this._buyerPoNumber, value, "BuyerPoNumber");
        }

        public string RMA_Number {
            get => this._rmaNumber;
            set => SetProperty(ref this._rmaNumber, value, "RMA_Number");
        }

        public ICommand RemoveFromOutgoingCommand {
            get => this.RemoveFromOutgoingDelegate;
        }

        private void RemoveFromOutgoingHandler() {
            if(this.SelectedRank != null) {
                this.OutgoingList.Remove(this.SelectedRank);
                RaisePropertyChanged("OutgoingList");
            }
        }

        private void AddToOutgoingHandler(ProductInstance rank) {
            if(rank != null) {
                this.OutgoingList.Add(rank);
                RaisePropertyChanged("OutgoingList");
            }
        }

        private void PopulateConsumers() {
            this.Consumers = this._dataManager.ConsumerProvider.GetEntityList().ToList();
        }

        private void CheckOutHandler() {
            if(this.OutgoingList != null) {
                if(this.SelectedConsumer != null) {
                    var responce = this._dataManager.Checkout(this.OutgoingList, this.SelectedConsumer, this.BuyerPoNumber, this.RMA_Number);
                    if(responce.Success) {
                        this.MessageBoxService.ShowMessage(responce.Message, "Success", MessageButton.OK, MessageIcon.Information);
                        this.OutgoingList.Clear();
                        this._dataManager.UpdateProductTotals();
                        this._eventAggregator.GetEvent<DoneOutgoingListEvent>().Publish();
                    } else {
                        this.MessageBoxService.ShowMessage(responce.Message, "Error", MessageButton.OK, MessageIcon.Error);
                    }
                } else {
                    this.MessageBoxService.ShowMessage("Consumer Not Selected, Please Make Selection and Try Again", "Error", MessageButton.OK, MessageIcon.Error);
                }
            }
        }

        private void CancelHandler() {
            this._eventAggregator.GetEvent<CancelOutgoingListEvent>().Publish();
            this.OutgoingList.Clear();
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var rank = navigationContext.Parameters["Rank"] as ProductInstance;
            if(rank != null) {
                this.OutgoingList.Add(rank);
                RaisePropertyChanged("OutgoingList");
            }
            this._eventAggregator.GetEvent<StartOutgoingListEvent>().Publish();
        }
    }
}