using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Inventory.Common.BuisnessLayer;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;

namespace Inventory.Common.DataLayer.Providers {
    public class PartInstanceProvider : IEntityDataProvider<PartInstance> {
        public PartInstance GetEntity(string entityName) => throw new NotImplementedException();
        public PartInstance GetEntity(Expression<Func<PartInstance, bool>> expression) => throw new NotImplementedException();
        public IEnumerable<PartInstance> GetEntityList(Expression<Func<PartInstance, bool>> expression = null, Func<IQueryable<PartInstance>, IOrderedQueryable<PartInstance>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<PartInstance>> GetEntityListAsync(Expression<Func<PartInstance, bool>> expression = null, Func<IQueryable<PartInstance>, IOrderedQueryable<PartInstance>> orderBy = null) => throw new NotImplementedException();
        public void LoadData() => throw new NotImplementedException();
        public Task LoadDataAsync() => throw new NotImplementedException();
    }

    public class ProductInstanceProvider : IEntityDataProvider<ProductInstance> {
        private InventoryContext _context;
        private IUserService _userService;

        public ProductInstanceProvider(InventoryContext context, IUserService userService) {
            this._context = context;
            this._userService = userService;
        }

        public ProductInstance GetEntity(string entityName) {
            return this._context.Instances.OfType<ProductInstance>().AsNoTracking()
                .Include(e => e.CurrentLocation)
                .Include(e => e.Lot.Cost)
                .Include(e => e.InventoryItem)
                .Include(e => e.ProductReservations.Select(x => x.User))
                .Include(e => e.Transactions.Select(x => x.Session.User))
                .Include(e => e.Transactions.Select(x => x.Location))
                .FirstOrDefault(x => x.Name == entityName);
        }

        public ProductInstance GetEntity(Expression<Func<ProductInstance, bool>> expression) {
            return this._context.Instances.OfType<ProductInstance>().AsNoTracking()
                .Include(e => e.CurrentLocation)
                .Include(e => e.Lot.Cost)
                .Include(e => e.ProductReservations.Select(x => x.User))
                .Include(e => e.Transactions.Select(x => x.Session.User))
                .Include(e => e.Transactions.Select(x => x.Location))
                .FirstOrDefault(expression);
        }

        public IEnumerable<ProductInstance> GetEntityList(Expression<Func<ProductInstance, bool>> expression = null,
            Func<IQueryable<ProductInstance>, IOrderedQueryable<ProductInstance>> orderBy = null) {

            IQueryable<ProductInstance> query = this._context.Set<ProductInstance>().AsNoTracking()
                .Include(e => e.CurrentLocation)
                .Include(e => e.Lot.Cost)
                .Include(e => e.InventoryItem)
                .Include(e => e.ProductReservations.Select(x => x.User))
                .Include(e => e.Transactions.Select(x => x.Location))
                .Include(e => e.Transactions.Select(x => x.Session.User));

            if(expression != null) {
                query = query.Where(expression);
            }

            if(orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<ProductInstance>> GetEntityListAsync(Expression<Func<ProductInstance, bool>> expression = null, 
            Func<IQueryable<ProductInstance>, IOrderedQueryable<ProductInstance>> orderBy = null) {

            IQueryable<ProductInstance> query = this._context.Set<ProductInstance>().AsNoTracking()
                .Include(e => e.CurrentLocation)
                .Include(e => e.InventoryItem)
                .Include(e => e.Lot.Cost)
                .Include(e => e.ProductReservations.Select(x=>x.User))
                .Include(e => e.Transactions.Select(x => x.Location))
                .Include(e => e.Transactions.Select(x=>x.Session.User));

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
            this._context.Instances.OfType<ProductInstance>().AsNoTracking()
                .Include(e => e.CurrentLocation)
                .Include(e=>e.InventoryItem)
                .Include(e => e.Lot.Cost)
                .Include(e => e.ProductReservations.Select(x => x.User))
                .Include(e => e.Transactions.Select(x => x.Location))
                .Include(e => e.Transactions.Select(x => x.Session.User))
                .Load();
        }

        public async Task LoadDataAsync() {
            await this._context.Instances.OfType<ProductInstance>().AsNoTracking()
                .Include(e => e.CurrentLocation)
                .Include(e => e.Lot.Cost)
                .Include(e => e.ProductReservations.Select(x => x.User))
                .Include(e => e.Transactions.Select(x => x.Location))
                .Include(e => e.Transactions.Select(x => x.Session.User))
                .LoadAsync();
        }
    }
}
