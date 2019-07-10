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
    public abstract class DataManagerBase:IDisposable {
        protected InventoryContext _context;
        protected IUserService _userService;
        protected object lockVaiable = new object();

        private bool disposed = false;

        protected DataManagerBase(InventoryContext context, IUserService userService) {
            this._context = context;
            this._userService = userService;
        }

        public void Commit() {
            this._context.SaveChanges();
        }

        public async Task CommitAsync() {
            await this._context.SaveChangesAsync();
        }

        public abstract void UndoChanges();

        protected virtual void Dispose(bool disposing) {
            if(!this.disposed) {
                if(disposing) {
                    this._context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
