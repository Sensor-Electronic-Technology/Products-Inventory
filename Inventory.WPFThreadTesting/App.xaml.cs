using Inventory.WPFThreadTesting.Views;
using Inventory.Common.ApplicationLayer;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using Prism.DryIoc;
using Inventory.Common.EntityLayer.Model;
using DryIoc;
using Prism.Regions;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Prism;
using DevExpress.Xpf.Grid;

namespace Inventory.WPFThreadTesting
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<DXMainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e) {
            GridControl.AllowInfiniteGridSize = true;
            base.OnStartup(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var container = containerRegistry.GetContainer();
            container.Register<InventoryContext>(setup: Setup.With(allowDisposableTransient: true));
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings) {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            var factory = base.Container.Resolve<IRegionBehaviorFactory>();
            regionAdapterMappings.RegisterMapping(typeof(DocumentGroup),AdapterFactory.Make<RegionAdapterBase<DocumentGroup>>(factory));
            regionAdapterMappings.RegisterMapping(typeof(LayoutPanel), AdapterFactory.Make<RegionAdapterBase<LayoutPanel>>(factory));
            regionAdapterMappings.RegisterMapping(typeof(LayoutGroup), AdapterFactory.Make<RegionAdapterBase<LayoutGroup>>(factory));
            regionAdapterMappings.RegisterMapping(typeof(TabbedGroup), AdapterFactory.Make<RegionAdapterBase<TabbedGroup>>(factory));
        }
    }
}
