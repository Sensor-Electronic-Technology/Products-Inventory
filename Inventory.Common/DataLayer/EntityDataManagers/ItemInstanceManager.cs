using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;
using EntityFramework.Triggers;
using System.Linq.Expressions;
using Inventory.Common.ApplicationLayer;
using Inventory.Common.DataLayer.Providers;
using Inventory.Common.DataLayer.EntityOperations;
using Inventory.Common.BuisnessLayer;

namespace Inventory.Common.DataLayer.EntityDataManagers {
    public class PartInstanceManager : DataManagerBase {

        private IEntityDataOperations<ProductInstance> _productInstanceOperations;
        private IEntityDataProvider<ProductInstance> _productInstanceProvider;

        public PartInstanceManager(InventoryContext context, IUserService userService)
        : base(context, userService) {
            this._productInstanceOperations = new ProductInstanceOperations(this._context,this._userService);
            this._productInstanceProvider = new ProductInstanceProvider(this._context, this._userService);
        }

        public IEntityDataProvider<ProductInstance> ProductInstanceProvider {
            get => this._productInstanceProvider;
        }

        public IEntityDataOperations<ProductInstance> ProductInstanceOperations {
            get => this._productInstanceOperations;
        }

        public override void UndoChanges() {
            this._context.UndoDbContext();
        }
    }
}
