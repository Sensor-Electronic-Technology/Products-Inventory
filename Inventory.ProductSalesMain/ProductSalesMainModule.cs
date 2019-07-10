using Inventory.Common.ApplicationLayer;
using Inventory.ProductSalesMain.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Inventory.ProductSalesMain
{
    public class ProductSalesMainModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(Regions.MainWindow, typeof(ProductSalesMainView));
            regionManager.RegisterViewWithRegion(Regions.MainRegion, typeof(WelcomeView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterForNavigation(typeof(WelcomeView), AppViews.WelcomeView);
        }
    }
}