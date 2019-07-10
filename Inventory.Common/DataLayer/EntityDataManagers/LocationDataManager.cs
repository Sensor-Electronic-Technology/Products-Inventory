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
    public class LocationDataManager : DataManagerBase {

        IEntityDataProvider<Warehouse> _warehouseProvider;
        IEntityDataOperations<Warehouse> _warehouseOperations;
        IEntityDataProvider<Consumer> _consumerProvider;
        IEntityDataOperations<Consumer> _consumerOperations;

        public LocationDataManager(InventoryContext context, IUserService userService) : base(context, userService) {
            this._context = context;
            this._userService = userService;

            this._warehouseProvider = new WarehouseProvider(this._context, this._userService);
            this._warehouseOperations = new WarehouseOperations(this._context, this._userService); ;
            this._consumerOperations = new ConsumerOperations(this._context, this._userService);
            this._consumerProvider = new ConsumerProvider(this._context, this._userService);
        }

        public void CommitChanges() {
            this._context.SaveChanges();
        }

        public IEntityDataProvider<Warehouse> WarehouseProvider {
            get => this._warehouseProvider;
        }

        public IEntityDataOperations<Warehouse> WarehouseOperations {
            get => this._warehouseOperations;
        }

        public IEntityDataProvider<Consumer> ConsumerProvider {
            get => this._consumerProvider;
        }

        public IEntityDataOperations<Consumer> ConsumerOperations {
            get => this._consumerOperations;
        }

        public override void UndoChanges() {
            this._context.UndoDbEntries<Warehouse>();
            this._context.UndoDbEntries<Consumer>();
            this._context.UndoDbEntries<Location>();
        }
    }
}