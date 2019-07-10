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
using Inventory.Common.BuisnessLayer;
using Inventory.Common.DataLayer.EntityOperations;

namespace Inventory.Common.DataLayer.EntityDataManagers {

    public class UserManager:DataManagerBase {

        private IDomainManager _domainmanager;

        private IEntityDataProvider<User> _inventoryUserProvider;
        private IEntityDataProvider<User> _domainUserProvider;
        private IEntityDataOperations<User> _inventoryUserOperations;
        private IEntityDataOperations<User> _domainUserOperations;

        public UserManager(InventoryContext context,IDomainManager domainManager, IUserService userService):base(context,userService) {
            this._context = context;
            this._userService = userService;
            this._domainmanager = domainManager;

            this._inventoryUserProvider = new InventoryUserProvider(this._context, this._domainmanager, this._userService);
            this._domainUserProvider = new DomainUserProvider(this._context, this._domainmanager, this._userService);
            this._inventoryUserOperations = new InventoryUserDataOperations(this._context, this._domainmanager, this._userService);
            this._domainUserOperations = new DomainUserDataOperations(this._context, this._domainmanager, this._userService);
        }

        public IEntityDataProvider<User> InventoryUserProvider {
            get => this._inventoryUserProvider;
        }

        public IEntityDataProvider<User> DomainUserProvider {
            get => this._domainUserProvider;
        }

        public IEntityDataOperations<User> InventoryUserOperations {
            get => this._inventoryUserOperations;
        }

        public IEntityDataOperations<User> DomainUserOperations {
            get => this._domainUserOperations;
        }

        public override void UndoChanges() {
            this._context.UndoDbEntries<User>();
            this._context.UndoDbEntries<Permission>();
        }
    }
}
