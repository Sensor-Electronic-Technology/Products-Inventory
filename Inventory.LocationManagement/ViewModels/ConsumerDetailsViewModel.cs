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
    public class ConsumerDetailsViewModel : InventoryViewModelNavigationBase {

        private IEventAggregator _eventAggregator;

        private Consumer _selectedConsumer = new Consumer();
        private bool _isNew = false;

        public PrismCommands.DelegateCommand SaveCommand { get; private set; }
        public PrismCommands.DelegateCommand DiscardCommand { get; private set; }

        public ConsumerDetailsViewModel(IEventAggregator eventAggregator) {
            this._eventAggregator = eventAggregator;
            this.SaveCommand = new PrismCommands.DelegateCommand(this.SaveHandler);
            this.DiscardCommand = new PrismCommands.DelegateCommand(this.DiscardHandler);
        }

        public override bool KeepAlive {
            get => false;
        }

        public Consumer SelectedConsumer {
            get => this._selectedConsumer;
            set => SetProperty(ref this._selectedConsumer, value, "SelectedConsumer");
        }

        public void SaveHandler() {
            if(this._isNew) {
                this._eventAggregator.GetEvent<ConsumerSaveEvent>().Publish(this.SelectedConsumer);
            } else {
                this._eventAggregator.GetEvent<ConsumerUpdateEvent>().Publish(this.SelectedConsumer);
            }
        }

        public void DiscardHandler() {
            this._eventAggregator.GetEvent<ConsumerDiscardEvent>().Publish();
        }


        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            var location = navigationContext.Parameters["Consumer"] as Consumer;
            if(location != null) {
                return this.SelectedConsumer != null && this.SelectedConsumer.Name == location.Name;
            } else {
                return true;
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var location = navigationContext.Parameters["Consumer"] as Consumer;
            var isNew = Convert.ToBoolean(navigationContext.Parameters["IsNew"]);
            if(location != null) {
                this.SelectedConsumer = location;
                this._isNew = isNew;
            }
        }
    }
}