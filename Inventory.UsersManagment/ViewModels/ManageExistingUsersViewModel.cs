using DevExpress.Mvvm;
using Inventory.Common.EntityLayer.Model.Entities;
using Prism.Events;
using System.Collections.Generic;
using Inventory.Common.ApplicationLayer;
using System.Linq;
using Prism.Regions;
using Prism.Ioc;
using Inventory.Common.BuisnessLayer;
using Inventory.Common.DataLayer.EntityDataManagers;

namespace Inventory.UsersManagment.ViewModels {
    public class ManageExistingUsersViewModel : Prism.Mvvm.BindableBase, DevExpress.Mvvm.ISupportServices, IRegionMemberLifetime {
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
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("ManageExistingUsersNotification"); } }


        //Private Property Fields
        private List<User> _inventoryUsers = new List<User>();
        private User _selectedInventoryUser = new User();

        //Delegates
        public Prism.Commands.DelegateCommand LoadedCommand { get; private set; }
        public Prism.Commands.DelegateCommand<object> InventoryUserSelectedCommand { get; private set; }
        public Prism.Commands.DelegateCommand<object> DeleteCommand { get; private set; }

        //Properties
        public List<User> InventoryUsers {
            get => _inventoryUsers;
            set => SetProperty(ref this._inventoryUsers, value, "InventoryUsers");
        }

        public User SelectedInventoryUser {
            get => this._selectedInventoryUser;
            set => SetProperty(ref this._selectedInventoryUser, value, "SelectedInventoryUser");
        }

        public bool KeepAlive { get => false; }

 /*       public ManageExistingUsersViewModel(IContainerExtension container, IUserService userService, IEventAggregator eventAggregator, IRegionManager rm) {
            //Services'
            this._userDataManager = container.Resolve<IEntityDataManager<User>>(AppViews.ManageExistingUsersView);
            this._userService = userService;
            this._eventAggregator = eventAggregator;
            this._regionManager = rm;

            //Commands
            this.LoadedCommand = new Prism.Commands.DelegateCommand(this.PopulateAsync);
            this.InventoryUserSelectedCommand = new Prism.Commands.DelegateCommand<object>(this.UserSelectedHandler);
            this.DeleteCommand = new Prism.Commands.DelegateCommand<object>(this.DeleteUserHandler);

            //Event Aggregator
            this._eventAggregator.GetEvent<UserUpdatedEvent>().Subscribe(this.UserUpdatedHandler, ThreadOption.UIThread);

            //Populate Data
            //this.Populate();
            this.PopulateAsync();
        }*/

        public ManageExistingUsersViewModel(UserManager userManager, IUserService userService, IEventAggregator eventAggregator, IRegionManager rm) {
            //Services'
            this._userDataManager = userManager;
            this._userService = userService;
            this._eventAggregator = eventAggregator;
            this._regionManager = rm;

            //Commands
            this.LoadedCommand = new Prism.Commands.DelegateCommand(this.PopulateAsync);
            this.InventoryUserSelectedCommand = new Prism.Commands.DelegateCommand<object>(this.UserSelectedHandler);
            this.DeleteCommand = new Prism.Commands.DelegateCommand<object>(this.DeleteUserHandler);

            //Event Aggregator
            this._eventAggregator.GetEvent<UserUpdatedEvent>().Subscribe(this.UserUpdatedHandler, ThreadOption.UIThread);

            //Populate Data
            //this.Populate();
            this.PopulateAsync();
        }

        //Event Handlers and Methods
        public void Populate() {
            this.InventoryUsers = this._userDataManager.InventoryUserProvider.GetEntityList().ToList();
        }

        public async void PopulateAsync() {
            //await this._userDataManager.InventoryUserProvider.LoadDataAsync(); 
            this.InventoryUsers = (await this._userDataManager.InventoryUserProvider.GetEntityListAsync()).ToList();
        }

        private void DeleteUserHandler(object user) {
            if(user != null) {
                var userName = ((User)user).UserName;
                var deletedUser = this._userDataManager.InventoryUserOperations.Delete((User)user);
                if(deletedUser!=null) {
                    this._userDataManager.Commit();
                    this.PopulateAsync();
                    this.MessageBoxService.ShowMessage("User: " + userName + " Deleted");
                } else {
                    this._userDataManager.UndoChanges();
                    this.PopulateAsync();
                    this.MessageBoxService.ShowMessage("Delete Failed, No Changed Made");
                }
            }
        }

        private void UserUpdatedHandler(User user) {
            if(user != null) {
                var savedUser = this._userDataManager.InventoryUserOperations.Update(user);
                if(savedUser!=null) {
                    this._userDataManager.Commit();
                    this.PopulateAsync();
                    this.MessageBoxService.ShowMessage("User: " + user.UserName + " saved");
                } else {
                    this._userDataManager.UndoChanges();
                    this.PopulateAsync();
                    this.MessageBoxService.ShowMessage("Save Failed, No Changes Made");
                }
            }
        }

        private void UserSelectedHandler(object user) {
            if(user != null) {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("User", this.SelectedInventoryUser);
                parameters.Add("From", AppViews.ManageExistingUsersView);
                this._regionManager.RequestNavigate(Regions.UserDetailsRegion, AppViews.UserDetailsView, parameters);
            }
        }

    }
}