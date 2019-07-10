using System;
using System.Collections.Generic;
using System.Linq;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.Common.BuisnessLayer;
using Inventory.Common.ApplicationLayer;
using DevExpress.Mvvm;
using Prism.Regions;
using Prism.Ioc;
using Prism.Events;
using PrismCommands = Prism.Commands;
using System.Threading.Tasks;
using Inventory.Common.DataLayer.EntityDataManagers;
using System.Windows;

namespace Inventory.ProductsManagment.ViewModels {
    public class ProductsMainViewModel : InventoryViewModelBase {

        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;

        public ProductsMainViewModel(IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
        }

        public override bool KeepAlive {
            get => true;
        }
    }
}