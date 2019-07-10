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
    public class LotProvider : IEntityDataProvider<Lot> {
        private InventoryContext _context;
        private IUserService _userService;

        public LotProvider(InventoryContext context,IUserService userService) {
            this._context = context;
            this._userService = userService;
        }


        public Lot GetEntity(string entityName) {
            return null;
        }

        public Lot GetEntity(Expression<Func<Lot, bool>> expression) {
            return this._context.Lots.AsNoTracking()
                .Include(e => e.Attachments)
                .Include(e => e.Cost)
                //.Include(e => e.Transactions.Select(x => x.Session))
                //.Include(e => e.Transactions.Select(x => x.Instance))
                .Include(e => e.ProductInstances.Select(x => x.ProductReservations))
                .FirstOrDefault(expression);
        }

        public IEnumerable<Lot> GetEntityList(Expression<Func<Lot, bool>> expression = null, 
            Func<IQueryable<Lot>, IOrderedQueryable<Lot>> orderBy = null) {

            IQueryable<Lot> query = this._context.Set<Lot>().AsNoTracking()
                .Include(e => e.Attachments)
                .Include(e => e.Cost)
                //.Include(e => e.Transactions.Select(x => x.Session))
                //.Include(e => e.Transactions.Select(x => x.Instance))
                .Include(e => e.ProductInstances.Select(x => x.ProductReservations));

            if(expression != null) {
                query = query.Where(expression);
            }

            if(orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<Lot>> GetEntityListAsync(Expression<Func<Lot, bool>> expression = null, Func<IQueryable<Lot>, IOrderedQueryable<Lot>> orderBy = null) {
            IQueryable<Lot> query = this._context.Set<Lot>().AsNoTracking()
                .Include(e => e.Attachments)
                .Include(e => e.Cost)
                //.Include(e => e.Transactions.Select(x => x.Session))
                //.Include(e => e.Transactions.Select(x => x.Instance))
                .Include(e => e.ProductInstances.Select(x => x.ProductReservations));
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
            this._context.Lots
                .Include(e => e.Attachments)
                .Include(e => e.Cost)
                //.Include(e => e.Transactions.Select(x => x.Session))
                //.Include(e => e.Transactions.Select(x => x.Instance))
                .Include(e => e.ProductInstances.Select(x => x.ProductReservations))
                .Load();
        }

        public async Task LoadDataAsync() {
            await this._context.Lots
                .Include(e => e.Attachments)
                .Include(e => e.Cost)
                //.Include(e => e.Transactions.Select(x => x.Session))
                //.Include(e => e.Transactions.Select(x => x.Instance))
                .Include(e => e.ProductInstances.Select(x => x.ProductReservations))
                .LoadAsync();
        }
    }
}
