using Prism.Regions;
using DevExpress.Mvvm;
using Inventory.Common.ApplicationLayer;
using DevExpress.Xpf.Core;
using Inventory.Common.BuisnessLayer;

namespace Inventory.ProductSalesMain.ViewModels {
    public class ProductSalesMainViewModel : InventoryViewModelBase {

        private IRegionManager _regionManager;
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("Notice"); } }

        private bool _userManagmentEnabled = false;

        public Prism.Commands.DelegateCommand<string> LoadModuleCommand { get; private set; }
        public Prism.Commands.DelegateCommand OnLoadedCommand { get; private set; }

        public ProductSalesMainViewModel(IRegionManager regionManager) {
            this._regionManager = regionManager;
            this.LoadModuleCommand = new Prism.Commands.DelegateCommand<string>(this.LoadModuleHandler);
            this.OnLoadedCommand = new Prism.Commands.DelegateCommand(this.OnLoadedHandler);
            this.LoadModuleHandler(AppViews.WelcomeView);
            this.SetAvailable();
        }

        public override bool KeepAlive {
            get => true;
        }

        public bool UserManagmentEnabled {
            get => this._userManagmentEnabled;
            set => SetProperty(ref this._userManagmentEnabled, value, "UserManagmentEnabled");
        }

        private void SetAvailable() {
            this.UserManagmentEnabled = true;
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