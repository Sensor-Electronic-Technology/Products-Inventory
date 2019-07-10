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

namespace Inventory.LocationManagement.ViewModels {
    public class LocationMainViewModel : InventoryViewModelBase {

        private IRegionManager _regionManager;
        public PrismCommands.DelegateCommand<string> LoadLocationViewCommand { get; private set; }

        public LocationMainViewModel(IRegionManager regionManager) {
            this._regionManager = regionManager;
            this.LoadLocationViewCommand = new Prism.Commands.DelegateCommand<string>(this.LoadLocationViewHandler);
        }

        public override bool KeepAlive {
            get => true;
        }

        private void LoadLocationViewHandler(string navigationPath) {
            if(!string.IsNullOrEmpty(navigationPath)) {
                this._regionManager.RequestNavigate(Regions.LocationTableRegion, navigationPath);
            }
        }
    }
}