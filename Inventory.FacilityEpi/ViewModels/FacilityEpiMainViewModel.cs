using System;
using Prism.Regions;
using Prism.Events;
using Prism.Ioc;
using DevExpress.Mvvm;
using Inventory.Common.ApplicationLayer;
using DevExpress.Xpf.Core;

namespace Inventory.FacilityEpi.ViewModels {
    public class FacilityEpiMainViewModel : InventoryViewModelBase {

        private IRegionManager _regionManager;
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("Notice"); } }

        public Prism.Commands.DelegateCommand<string> LoadModuleCommand { get; private set; }
        public Prism.Commands.DelegateCommand OnLoadedCommand { get; private set; }

        public FacilityEpiMainViewModel(IRegionManager regionManager) {
            this._regionManager = regionManager;
            this.LoadModuleCommand = new Prism.Commands.DelegateCommand<string>(this.LoadModuleHandler);
            this.OnLoadedCommand = new Prism.Commands.DelegateCommand(this.OnLoadedHandler);
        }

        public override bool KeepAlive {
            get => true;
        }

        private void OnLoadedHandler() {
            if(DXSplashScreen.IsActive)
                DXSplashScreen.Close();
        }

        private void LoadModuleHandler(string navigationPath) {
            if(!string.IsNullOrEmpty(navigationPath)) {
                this._regionManager.RequestNavigate(Regions.MainRegion, navigationPath);
            }
        }
    }
}