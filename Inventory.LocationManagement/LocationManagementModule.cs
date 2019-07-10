using Inventory.LocationManagement.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Inventory.Common.ApplicationLayer;

namespace Inventory.LocationManagement
{
    public class LocationManagementModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<WarehouseTableView>(AppViews.WarehouseTableView);
            containerRegistry.RegisterForNavigation<ConsumerTableView>(AppViews.ConsumerTableView);
            containerRegistry.RegisterForNavigation<ConsumerDetailsView>(AppViews.ConsumerDetailsView);
            containerRegistry.RegisterForNavigation<LocationMainView>(AppViews.LocationMainView);
            containerRegistry.RegisterForNavigation<WarehouseDetailsView>(AppViews.WarehouseDetailsView);
        }
    }
}