using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using Inventory.Common.EntityLayer.Model.Entities;
using Prism.Events;
using Prism.Regions;
using Inventory.Common.ApplicationLayer;
using Inventory.Common.DataLayer.Providers;
using Inventory.Common.BuisnessLayer;

namespace Inventory.UsersManagment.ViewModels {
    public class UserDetailsViewModel : Prism.Mvvm.BindableBase, DevExpress.Mvvm.ISupportServices, INavigationAware, IRegionMemberLifetime {

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

        private IUserService _userService;
        private IEventAggregator _eventAggregator;
        private IEntityDataProvider<Permission> _permissionProvider;
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("UserDetailsNotification"); } }


        private User _selectedUser = new User();
        private Permission _selectedPermission = new Permission();
        private List<Permission> _permissions = new List<Permission>();
        private bool IsNewUser;
        private string _saveButtonName;
        private string _cancelButtonName;


        public Prism.Commands.DelegateCommand SaveChangesCommand { get; private set; }
        public Prism.Commands.DelegateCommand CancelChangesCommand { get; private set; }

        public bool KeepAlive {
            get => false;
        }

        public UserDetailsViewModel(IUserService userService, IEventAggregator eventAggregator, IEntityDataProvider<Permission> permissionProvider) {
            this._eventAggregator = eventAggregator;
            this._userService = userService;
            this._permissionProvider = permissionProvider;

            this.SaveChangesCommand = new Prism.Commands.DelegateCommand(() => Task.Run(() => this.SaveChangesHandler()));
            this.CancelChangesCommand = new Prism.Commands.DelegateCommand(() => Task.Run(() => this.CancelChangesHandler()));
            this.Populate();
        }

        public string SaveButtonName {
            get => this._saveButtonName;
            set => SetProperty(ref this._saveButtonName, value, "SaveButtonName");
        }

        public string CancelButtonName {
            get => this._cancelButtonName;
            set => SetProperty(ref this._cancelButtonName, value, "CancelButtonName");
        }

        public User SelectedUser {
            get => this._selectedUser;
            set => SetProperty(ref this._selectedUser, value, "SelectedUser");
        }

        public List<Permission> Permissions {
            get => this._permissions;
            set => SetProperty(ref this._permissions, value, "Permissions");
        }

        public Permission SelectedPermission {
            get => this._selectedPermission;
            set => SetProperty(ref this._selectedPermission, value, "SelectedPermission");
        }

        public void Populate() {
            this.Permissions = this._permissionProvider.GetEntityList().ToList();
            if(this.SelectedUser != null && this.SelectedUser.Permission != null) {
                var permission = this.Permissions.FirstOrDefault(e => e.Name == this.SelectedUser.Permission.Name);
                this.SelectedPermission = (permission != null) ? permission : this.Permissions[0];
            } else {
                this.SelectedPermission = this.Permissions[0];
            }
        }

        private void SaveChangesHandler() {
            if(this.SelectedPermission != null) {
                if(this.IsNewUser) {
                    this.SelectedUser.Permission = this.SelectedPermission;
                    this._eventAggregator.GetEvent<CreateNewUserEvent>().Publish(this.SelectedUser);
                } else {
                    this.SelectedUser.Permission = this.SelectedPermission;
                    this._eventAggregator.GetEvent<UserUpdatedEvent>().Publish(this.SelectedUser);
                }
            } else {
                this.MessageBoxService.ShowMessage("Error: No Permission Selected");
            }
        }

        private void CancelChangesHandler() {
            var permission = this.Permissions.FirstOrDefault(e => e.Name == this.SelectedUser.Permission.Name);
            this.SelectedPermission = (permission != null) ? permission : this.Permissions[0];
        }

        public void OnNavigatedTo(NavigationContext navigationContext) {
            var user = navigationContext.Parameters["User"] as User;
            var from = navigationContext.Parameters["From"] as string;
            if(user != null && !string.IsNullOrEmpty(from)) {
                this.SelectedUser = user;
                if(this.SelectedUser != null) {
                    if(this.SelectedUser.Permission != null) {
                        var permission = this.Permissions.FirstOrDefault(e => e.Name == this.SelectedUser.Permission.Name);
                        this.SelectedPermission = (permission != null) ? permission : this.Permissions[0];
                    }
                    if(from == AppViews.ConfigureNewUserView) {
                        this.SaveButtonName = "Save New Inventory User";
                        this.CancelButtonName = "Cancel New Inventory User";
                        this.IsNewUser = true;
                    } else if(from == AppViews.ManageExistingUsersView) {
                        this.SaveButtonName = "Save Changes";
                        this.CancelButtonName = "Cancel Changes";
                        this.IsNewUser = false;
                    }
                }

            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) {
            var user = navigationContext.Parameters["User"] as User;
            var from = navigationContext.Parameters["From"] as string;
            if(user != null && !string.IsNullOrEmpty(from)) {
                return this.SelectedUser != null && this.SelectedUser.UserName == user.UserName;
            } else {
                return true;
            }
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) {

        }
    }
}