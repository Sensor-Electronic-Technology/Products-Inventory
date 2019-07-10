using Inventory.FacilityEpi.Views;
using Inventory.Common.ApplicationLayer;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Inventory.FacilityEpi
{
    public class FacilityEpiModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(Regions.MainWindow,typeof(FacilityEpiMain));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}