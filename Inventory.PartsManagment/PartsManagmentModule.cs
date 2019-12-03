using Inventory.PartsManagment.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Inventory.Common.ApplicationLayer;

namespace Inventory.PartsManagment
{
    public class PartsManagmentModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider){
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(Regions.PartSelectorRegion, typeof(PartsSelectorView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry){
            containerRegistry.RegisterForNavigation<PartsMainView>();
            containerRegistry.RegisterForNavigation<PartDetailsView>();
            containerRegistry.RegisterForNavigation<PartInstanceDetailsView>();
        }
    }
}