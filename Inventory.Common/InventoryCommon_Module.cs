using Prism.Ioc;
using Prism.Modularity;
using Inventory.Common.DataLayer.Services;
using Inventory.Common.DataLayer;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.Common.EntityLayer.Model;

namespace Inventory.Common {
    public class InventoryCommon_Module : IModule {

        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.Register<IFileService,FileService>();

        }
    }
}
