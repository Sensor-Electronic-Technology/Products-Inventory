namespace Inventory.UsersManagment.ViewModels {
    using DevExpress.Mvvm;
    using Inventory.Common.ApplicationLayer;
    using Prism.Regions;

    public class UserManagmentViewModel : Prism.Mvvm.BindableBase, DevExpress.Mvvm.ISupportServices {
        public IServiceContainer _serviceContainer = null;
        IServiceContainer ISupportServices.ServiceContainer { get { return ServiceContainer; } }
        protected IServiceContainer ServiceContainer {
            get {
                if(this._serviceContainer == null) {
                    this._serviceContainer = new ServiceContainer(this);
                }
                return this._serviceContainer;
            }
        }

        private IRegionManager _regionManager;

        public Prism.Commands.DelegateCommand<string> LoadUserManagmentView { get; private set; }

        public UserManagmentViewModel(IRegionManager regionManager) {
            this._regionManager = regionManager;
            this.LoadUserManagmentView = new Prism.Commands.DelegateCommand<string>(this.LoadUserManagmentViewHandler);
        }

        private void LoadUserManagmentViewHandler(string navigationPath) {
            if(!string.IsNullOrEmpty(navigationPath)) {
                this._regionManager.RequestNavigate(Regions.UserManagmentRegion, navigationPath);
            }
        }


    }
}