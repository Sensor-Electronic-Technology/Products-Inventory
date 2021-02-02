using Inventory.ProductsManagment.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Inventory.Common.ApplicationLayer;

namespace Inventory.ProductsManagment
{
    public class ProductsManagmentModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(Regions.ProductSelectorRegion,typeof(ProductSelectorView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ProductsMainView>(AppViews.ProductsMainView);
            containerRegistry.RegisterForNavigation<ProductsLotRankView>(AppViews.ProductsLotRankView);
            containerRegistry.RegisterForNavigation<RankDetailsView>(AppViews.RankDetailsView);
            containerRegistry.RegisterForNavigation<LotDetailsView>(AppViews.LotDetailsView);
            containerRegistry.RegisterForNavigation<ProductReservationView>(AppViews.ProductReservationView);
            containerRegistry.RegisterForNavigation<OutgoingProductListView>(AppViews.OutgoingProductListView);
            containerRegistry.RegisterForNavigation<IncomingProductFormView>(AppViews.IncomingProductFormView);
            containerRegistry.RegisterForNavigation<IncomingProductListView>(AppViews.IncomingProductListView);
            containerRegistry.RegisterForNavigation<ImportLotsView>(AppViews.ImportLotsView);
        }
    }
}