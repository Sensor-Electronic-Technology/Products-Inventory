using Inventory.Common.ApplicationLayer;
using Inventory.CategoriesManagment.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Inventory.CategoriesManagment
{
    public class CategoriesManagmentModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ProductTypeTableView>(AppViews.ProductTypeTableView);
            containerRegistry.RegisterForNavigation<CategoryDetailsView>(AppViews.CategoryDetailsView);
            containerRegistry.RegisterForNavigation<CategoryMainView>(AppViews.CategoryMainView);
        }
    }
}