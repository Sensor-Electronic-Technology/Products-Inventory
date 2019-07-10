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
    public class IncomingInstructionsViewModel : InventoryViewModelNavigationBase {
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        //private ProductDataManager _dataManager;
        //public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("IncomingProductListNotifications"); } }
        //public IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("IncomingProductDispatcher"); } }

        public IncomingInstructionsViewModel(IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
        }

        public override bool KeepAlive {
            get => false;
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) => throw new NotImplementedException();
        public override void OnNavigatedFrom(NavigationContext navigationContext) => throw new NotImplementedException();
        public override void OnNavigatedTo(NavigationContext navigationContext) => throw new NotImplementedException();
    }
}