using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm;
using Inventory.Common.ApplicationLayer;
using Inventory.Common.EntityLayer.Model.Entities;
using Prism.Events;
using Prism.Regions;
using Inventory.Common.BuisnessLayer;
using Prism.Ioc;
using Inventory.Common.DataLayer.EntityDataManagers;

namespace Inventory.UsersManagment.ViewModels {
    public class ConfigureNewUserViewModel : Prism.Mvvm.BindableBase, DevExpress.Mvvm.ISupportServices, IRegionMemberLifetime {
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

        //Services
        private IUserService _userService;
        private UserManager _userDataManager;
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("ConfigureDomainUserNotification"); } }

        //Private Property Fields
        private List<User> _domainUsers = new List<User>();
        private User _selectedUser = new User();

        //Delegates
        public Prism.Commands.DelegateCommand LoadedCommand { get; private set; }
        public Prism.Commands.DelegateCommand<object> DomainUserSelectedCommand { get; private set; }
        public Prism.Commands.DelegateCommand<object> DeleteCommand { get; private set; }

        //Properties
        public List<User> DomainUsers {
            get => _domainUsers;
            set => SetProperty(ref this._domainUsers, value, "DomainUsers");
        }

        public User SelectedUser {
            get => this._selectedUser;
            set => SetProperty(ref this._selectedUser, value, "SelectedUser");
        }

        public bool KeepAlive { get => false; }

 /*       public ConfigureNewUserViewModel(IContainerExtension container, IUserService userService, IEventAggregator eventAggregator, IRegionManager rm) {

            //Services
            this._userDataManager = container.Resolve<IEntityDataManager<User>>(AppViews.ConfigureNewUserView);
            this._userService = userService;
            this._eventAggregator = eventAggregator;
            this._regionManager = rm;

            //Commands
            this.LoadedCommand = new Prism.Commands.DelegateCommand(this.PopulateAsync);
            this.DomainUserSelectedCommand = new Prism.Commands.DelegateCommand<object>(this.UserSelectedHandler);
            this.DeleteCommand = new Prism.Commands.DelegateCommand<object>(this.DeleteUserHandler);

            this._eventAggregator.GetEvent<CreateNewUserEvent>().Subscribe(this.CreateNewUserHandler, ThreadOption.UIThread);

            //Populate Data
            this.PopulateAsync();
        }*/

        public ConfigureNewUserViewModel(UserManager userManager, IUserService userService, IEventAggregator eventAggregator, IRegionManager rm) {

            //Services
            this._userDataManager = userManager;
            this._userService = userService;
            this._eventAggregator = eventAggregator;
            this._regionManager = rm;

            //Commands
            this.LoadedCommand = new Prism.Commands.DelegateCommand(this.PopulateAsync);
            this.DomainUserSelectedCommand = new Prism.Commands.DelegateCommand<object>(this.UserSelectedHandler);
            this.DeleteCommand = new Prism.Commands.DelegateCommand<object>(this.DeleteUserHandler);

            this._eventAggregator.GetEvent<CreateNewUserEvent>().Subscribe(this.CreateNewUserHandler, ThreadOption.UIThread);

            //Populate Data
            this.PopulateAsync();
        }

        //Event Handlers and Methods
        public async void PopulateAsync() {
            this.DomainUsers = (await this._userDataManager.DomainUserProvider.GetEntityListAsync()).ToList();
        }

        public void Populate() {
            this.DomainUsers = this._userDataManager.DomainUserProvider.GetEntityList().ToList();
        }

        private void DeleteUserHandler(object user) {
            if(user != null) {
                var userName = ((User)user).UserName;
                var deletedUser=this._userDataManager.DomainUserOperations.Delete((User)user);
                if(deletedUser!=null) {
                    this._userDataManager.Commit();
                    this.MessageBoxService.ShowMessage("User: " + userName + " Deleted");
                    this.PopulateAsync();
                } else {
                    this._userDataManager.UndoChanges();
                    this.MessageBoxService.ShowMessage("Delete Failed, No Changed Made");
                    this.PopulateAsync();
                }
            }
        }

        private void UserSelectedHandler(object user) {
            if(user != null) {
                var userParam = this._userDataManager.DomainUserProvider.GetEntity(((User)user).UserName);
                if(userParam != null) {
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add("User", userParam);
                    parameters.Add("From", AppViews.ConfigureNewUserView);
                    this._regionManager.RequestNavigate(Regions.UserDetailsRegion, AppViews.UserDetailsView, parameters);
                }
            }
        }

        private void CreateNewUserHandler(User user) {
            if(user != null) {
                var savedUser = this._userDataManager.DomainUserOperations.Add(user);
                if(savedUser!=null) {
                    this._userDataManager.Commit();
                    this.MessageBoxService.ShowMessage("User: " + user.UserName + " saved");
                    this.PopulateAsync();
                } else {
                    this._userDataManager.UndoChanges();
                    this.MessageBoxService.ShowMessage("Save Failed, No Changes Made");
                    this.PopulateAsync();
                }
            }
        }
    }
}