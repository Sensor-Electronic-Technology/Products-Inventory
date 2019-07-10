using Inventory.Common.ApplicationLayer;
using Inventory.Reporting.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Inventory.Reporting
{
    public class ReportingModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ReportingMainView>(AppViews.ReportingMainView);
        }
    }
}