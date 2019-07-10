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

namespace Inventory.LocationManagement.ViewModels {

    public class WarehouseDetailsViewModel : InventoryViewModelNavigationBase {

        private IEventAggregator _eventAggregator;

        private Warehouse _selectedWarehouse = new Warehouse();
        private bool _isNew = false;

        public PrismCommands.DelegateCommand SaveCommand { get; private set; }
        public PrismCommands.DelegateCommand DiscardCommand { get; private set; }

        public WarehouseDetailsViewModel(IEventAggregator eventAggregator) {
            this._eventAggregator = eventAggregator;
            this.SaveCommand = new PrismCommands.DelegateCommand(this.SaveHandler);
            this.DiscardCommand = new PrismCommands.DelegateCommand(this.DiscardHandler);
        }

        public override bool KeepAlive {
            get => false;
        }

        public Warehouse SelectedWarehouse {
            get => this._selectedWarehouse;
            set => SetProperty(ref this._selectedWarehouse, value, "SelectedWarehouse");
        }

        public void SaveHandler() {
            if(this._isNew) {
                this._eventAggregator.GetEvent<WarehouseSaveEvent>().Publish(this.SelectedWarehouse);
            } else {
                this._eventAggregator.GetEvent<WarehouseUpdateEvent>().Publish(this.SelectedWarehouse);
            }
        }

        public void DiscardHandler() {
            this._eventAggregator.GetEvent<WarehouseDiscardEvent>().Publish();
        }


        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            var location = navigationContext.Parameters["Warehouse"] as Warehouse;
            if(location != null) {
                return this.SelectedWarehouse != null && this.SelectedWarehouse.Name == location.Name;
            } else {
                return true;
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var location = navigationContext.Parameters["Warehouse"] as Warehouse;
            var isNew = Convert.ToBoolean(navigationContext.Parameters["IsNew"]);
            if(location != null) {
                this.SelectedWarehouse = location;
                this._isNew = isNew;
            }
        }
    }
}