using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;

namespace Inventory.Common.DataLayer.Providers {
    public class PermissionProvider : IEntityDataProvider<Permission> {
        private InventoryContext _context;

        public PermissionProvider(InventoryContext context) {
            this._context = context;
        }

        public Permission GetEntity(string entityName) {
            return this._context.Permissions.AsNoTracking().FirstOrDefault(x => x.Name == entityName);
        }

        public Permission GetEntity(Expression<Func<Permission, bool>> expression) {
            return this._context.Permissions.AsNoTracking().FirstOrDefault(expression);
        }

        public IEnumerable<Permission> GetEntityList(Expression<Func<Permission, bool>> expression = null,
            Func<IQueryable<Permission>, IOrderedQueryable<Permission>> orderBy = null) {

            IQueryable<Permission> query = this._context.Set<Permission>().AsNoTracking();

            if(expression != null) {
                query = query.Where(expression);
            }

            if(orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<Permission>> GetEntityListAsync(Expression<Func<Permission, bool>> expression = null, 
            Func<IQueryable<Permission>, IOrderedQueryable<Permission>> orderBy = null) {
            IQueryable<Permission> query = this._context.Set<Permission>().AsNoTracking();

            if(expression != null) {
                query = query.Where(expression);
            }

            if(orderBy != null) {
                return await orderBy(query).ToListAsync();
            } else {
                return await query.ToListAsync();
            }
        }
        
        public void LoadData() {
            this._context.Permissions.Load();
        }

        public async Task LoadDataAsync() {
            await this._context.Permissions.LoadAsync();
        }
    }
}
