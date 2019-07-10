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
    public class CategoryDataManager : DataManagerBase {

        IEntityDataProvider<ProductType> _productTypeProvider;
        IEntityDataOperations<ProductType> _productTypeOperations;

        public CategoryDataManager(InventoryContext context, IUserService userService) 
            : base(context, userService) {
            this._productTypeProvider = new ProductTypeProvider(context, userService);
            this._productTypeOperations = new ProductTypeOperations(context, userService);
        }

        public IEntityDataProvider<ProductType> ProductTypeProvider {
            get => this._productTypeProvider;
        }

        public IEntityDataOperations<ProductType> ProductTypeOperations {
            get => this._productTypeOperations;
        }

        public override void UndoChanges() {
            this._context.UndoDbEntries<ProductType>();
        }
    }
}
