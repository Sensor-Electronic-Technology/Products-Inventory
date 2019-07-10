using Inventory.UsersManagment.Views;
using Inventory.Common.ApplicationLayer;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Inventory.UsersManagment
{
    public class UserManagmentModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var rm = containerProvider.Resolve<IRegionManager>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<UserManagmentView>(AppViews.UserManagmentView);
            containerRegistry.RegisterForNavigation<ConfigureNewUserView>(AppViews.ConfigureNewUserView);
            containerRegistry.RegisterForNavigation<ManageExistingUsersView>(AppViews.ManageExistingUsersView);
            containerRegistry.RegisterForNavigation<UserDetailsView>(AppViews.UserDetailsView);
        }
    }
}