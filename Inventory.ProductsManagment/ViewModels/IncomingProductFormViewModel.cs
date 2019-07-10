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
    public class IncomingProductFormViewModel : InventoryViewModelNavigationBase {

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private ProductDataManager _dataManager;
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("IncomingProductFormNotifications"); } }
        private Lot _lot = new Lot();
        private ProductInstance _rank = new ProductInstance();
        private ProductInstance _selectedRank = new ProductInstance();
        private Product _selectedProduct = new Product();
        private Cost _cost = new Cost();
        private ObservableCollection<ProductInstance> _ranks = new ObservableCollection<ProductInstance>();
        private string _setLot;
        private bool _canSetLot = true;
        private bool _lotInProgress = false;
        private string _rmaNumber;
        private double _unitCost;

        public PrismCommands.DelegateCommand AddLotToIncomingCommand { get; private set; }
        public PrismCommands.DelegateCommand AddRankToLotCommand { get; private set; }
        public PrismCommands.DelegateCommand CancelIncomingCommand { get; private set; }
        public PrismCommands.DelegateCommand SetLotCommand { get; private set; }
        public PrismCommands.DelegateCommand EditLotCommand { get; private set; }
        public ICommand RemoveRankFromLotCommand { get; private set; }

        public IncomingProductFormViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, ProductDataManager dataManager) {
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this._dataManager = dataManager;
            this.AddLotToIncomingCommand = new PrismCommands.DelegateCommand(this.AddLotToIncomingHandler);
            this.AddRankToLotCommand = new PrismCommands.DelegateCommand(this.AddRankToLotHandler);
            this.CancelIncomingCommand = new PrismCommands.DelegateCommand(this.CancelIncomingHandler);
            this.SetLotCommand = new PrismCommands.DelegateCommand(this.SetLotHandler);
            this.EditLotCommand = new PrismCommands.DelegateCommand(this.EditLotHandler);

            this._eventAggregator.GetEvent<SetInIncomingFormEvent>().Subscribe(this.SetProductHandler);
            this._eventAggregator.GetEvent<AddToIncomingCallback>().Subscribe(this.AddLotToIncomingCallBack);

            this.SetDefaults();
        }

        public override bool KeepAlive {
            get => true;
        }

        public Lot Lot {
            get => this._lot;
            set => SetProperty(ref this._lot,value,"Lot");
        }

        public ProductInstance Rank {
            get => this._rank;
            set => SetProperty(ref this._rank, value, "Rank");
        }

        public Product SelectedProduct {
            get => this._selectedProduct;
            set => SetProperty(ref this._selectedProduct, value, "SelectedProduct");
        }

        public ProductInstance SelectedRank {
            get => this._selectedRank;
            set => SetProperty(ref this._selectedRank, value, "SelectedRank");
        }

        public ObservableCollection<ProductInstance> Ranks {
            get => this._ranks;
            set => SetProperty(ref this._ranks,value,"Ranks");
        }

        public string SetLot {
            get => this._setLot;
            set => SetProperty(ref this._setLot, value, "SetLot");
        }

        public bool SetLotEnabled {
            get => this._canSetLot;
            set{
                SetProperty(ref this._canSetLot, value, "SetLotEnabled");
                RaisePropertyChanged("EditLotEnabled");
            } 
        }

        public bool EditLotEnabled {
            get => !this._canSetLot;
        }

        public string RmaNumber {
            get => this._rmaNumber;
            set => SetProperty(ref this._rmaNumber, value, "RmaNumber");
        }

        public double UnitCost {
            get => this._unitCost;
            set => SetProperty(ref this._unitCost, value, "UnitCost");
        }

        private void SetLotHandler() {
            bool validated= this.Lot.LotNumber != Constants.DefaultLotNumber
                && this.Lot.SupplierPoNumber != Constants.DefaultSupplierPo;
            if(validated) {
                this.SetLotEnabled = false;
                this.SetLot = "("+this.Lot.LotNumber + "," + this.Lot.SupplierPoNumber + ")";
                Cost newCost =new Cost();
                newCost.Amount = this.UnitCost;
                this.Lot.Cost = newCost;
                this.Lot.Product = this.SelectedProduct;
                this._lotInProgress = true;
            }
        }

        private void RemoveRankFromLotHandler() {
            if(this.SelectedRank != null) {
                if(!this.Ranks.Remove(this.SelectedRank)) {
                    this.MessageBoxService.ShowMessage("Error Removing Rank, Please check selection and try again", 
                        "Error", MessageButton.OK, MessageIcon.Error);
                } else {
                    RaisePropertyChanged("Lot");
                }
            }
        }

        private void EditLotHandler() {
            this.SetLotEnabled = true;
        }

        private void AddRankToLotHandler() {
            ProductInstance newRank = new ProductInstance();
            newRank.Name = this.Rank.Name;
            newRank.Quantity = this.Rank.Quantity;
            newRank.Wavelength = this.Rank.Wavelength;
            newRank.Power = this.Rank.Power;
            newRank.Voltage = this.Rank.Voltage;
            newRank.InventoryItem = this.SelectedProduct;
            this.Ranks.Add(newRank);
            this.SetRankDefaults();
        }

        private void AddLotToIncomingHandler() {
            Lot newLot = new Lot();
            newLot.LotNumber = this.Lot.LotNumber;
            newLot.SupplierPoNumber = this.Lot.SupplierPoNumber;
            newLot.Recieved = this.Lot.Recieved;
            newLot.Product = this.SelectedProduct;
            newLot.ProductName = this.SelectedProduct.Name;
            Cost newCost = new Cost();
            newCost.Amount = this.UnitCost;
            newLot.Cost = newCost;

            foreach(var rank in this.Ranks) {
                rank.Lot = newLot;
                newLot.ProductInstances.Add(rank);
            }
            IncomingLotCarrier carrier = new IncomingLotCarrier();
            carrier.Lot = newLot;
            carrier.RMA = string.IsNullOrEmpty(this.RmaNumber) ? "" : this.RmaNumber;
            this._eventAggregator.GetEvent<AddToIncomingEvent>().Publish(carrier);
        }

        private void SetProductHandler(Product product) {
            if(product != null) {
                if(this._lotInProgress) {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat("Lot({0},{1}) Not Added To Incoming ",this.Lot.LotNumber,this.Lot.SupplierPoNumber).AppendLine();
                    builder.AppendLine("Finish Adding Lot to Incoming or Continue");
                    builder.AppendLine("WARNING: IF YOU CONTINUE LOT INFORMATION WILL BE LOST. Continue?");
                    var responce=this.MessageBoxService.ShowMessage(builder.ToString(), "Warning", MessageButton.YesNo, MessageIcon.Warning, MessageResult.No);
                    if(responce == MessageResult.Yes) {
                        this.SelectedProduct = product;
                        this.SetDefaults();
                    }
                } else {
                    this.SelectedProduct = product;
                    this.SetDefaults();
                }
            }
        }

        private void AddLotToIncomingCallBack(bool success) {
            if(success) {
                this._lotInProgress = false;
                this.SetDefaults();
            } else {
                this.MessageBoxService.ShowMessage("Error Add Lot,Lot already Exist in Incoming List"+Environment.NewLine
                    +"Please Check Input And Try Again",
                    "Error", MessageButton.OK, MessageIcon.Error);
            }
        }

        private void CancelIncomingHandler() {
            this._eventAggregator.GetEvent<CancelIncomingListEvent>().Publish();
        }

        private void SetDefaults() {
            this.SetLotDefaults();
            this.SetRankDefaults();
            this.SetLotEnabled = true;
        }

        private void SetLotDefaults() {
            this.Lot.Recieved = DateTime.UtcNow;
            this.Lot.LotNumber = Constants.DefaultLotNumber;
            this.Lot.SupplierPoNumber = Constants.DefaultSupplierPo;
            this.UnitCost = 0.00;
            this.Lot.ProductInstances.Clear();
            this.Ranks.Clear();
            this.Lot.Product = this.SelectedProduct;
            RaisePropertyChanged("Lot");
            this.SetLot = "(Lot Not Set,Po Not Set)";
        }

        private void SetRankDefaults() {
            this.Rank.Name = Constants.DefaultRankName;
            this.Rank.Quantity = 0;
            this.Rank.Wavelength = "";
            this.Rank.Power = "";
            this.Rank.Voltage = "";
            this.Rank.InventoryItem = null;
            RaisePropertyChanged("Rank");
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
            if(product != null) {
                this.SelectedProduct = product;
                this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.IncomingProductListView);
            }
        }
    }
}