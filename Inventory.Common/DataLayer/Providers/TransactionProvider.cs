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
    public class ProductTransactionProvider : IEntityDataProvider<ProductTransaction> {
        private InventoryContext _context;
        private IUserService _userService;

        public ProductTransactionProvider(InventoryContext context, IUserService userService) {
            this._context = context;
            this._userService = userService;
        }

        public ProductTransaction GetEntity(string entityName) => throw new NotImplementedException();
        public ProductTransaction GetEntity(Expression<Func<ProductTransaction, bool>> expression) {
            return this._context.Transactions.OfType<ProductTransaction>()
                .AsNoTracking()
                .Include(e => e.Instance.InventoryItem)
                .Include(e => e.Location)
                .FirstOrDefault(expression);
        }

        public IEnumerable<ProductTransaction> GetEntityList(Expression<Func<ProductTransaction, bool>> expression = null, 
            Func<IQueryable<ProductTransaction>, IOrderedQueryable<ProductTransaction>> orderBy = null) {

            IQueryable<ProductTransaction> query = this._context.Set<ProductTransaction>()
                .AsNoTracking()
                .Include(e => e.Instance.InventoryItem)
                .Include(e => e.Location);

            if(expression != null) {
                query = query.Where(expression);
            }

            if(orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<ProductTransaction>> GetEntityListAsync(Expression<Func<ProductTransaction, bool>> expression = null, 
            Func<IQueryable<ProductTransaction>, IOrderedQueryable<ProductTransaction>> orderBy = null) {
            IQueryable<ProductTransaction> query = this._context.Set<ProductTransaction>()
                .AsNoTracking()
                .Include(e => e.Instance.InventoryItem)
                .Include(e => e.Location);

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
            this._context.Transactions.OfType<ProductTransaction>()
                .Include(e => e.Instance.InventoryItem)
                .Include(e => e.Location).Load();
        }

        public async Task LoadDataAsync() {
            await this._context.Transactions.OfType<ProductTransaction>()
                .Include(e => e.Instance.InventoryItem)
                .Include(e => e.Location).LoadAsync();
        }
    }
}
