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
    public class ReservationProvider : IEntityDataProvider<ProductReservation> {
        private InventoryContext _context;
        private IUserService _userService;

        public ReservationProvider(InventoryContext context, IUserService userService) {
            this._context = context;
            this._userService = userService;
        }

        public ProductReservation GetEntity(string entityName) {
            return null;
        }
        
        public ProductReservation GetEntity(Expression<Func<ProductReservation, bool>> expression) {
            return this._context.ProductReservations.Include(e => e.ProductInstance).Include(e => e.User).FirstOrDefault(expression);
        }

        public IEnumerable<ProductReservation> GetEntityList(Expression<Func<ProductReservation, bool>> expression = null, 
            Func<IQueryable<ProductReservation>, IOrderedQueryable<ProductReservation>> orderBy = null) {

            IQueryable<ProductReservation> query = this._context.Set<ProductReservation>().AsNoTracking().Include(e => e.ProductInstance).Include(e => e.User);

            if(expression != null) {
                query = query.Where(expression);
            }

            if(orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<ProductReservation>> GetEntityListAsync(Expression<Func<ProductReservation, bool>> expression = null, 
            Func<IQueryable<ProductReservation>, IOrderedQueryable<ProductReservation>> orderBy = null) {
            IQueryable<ProductReservation> query = this._context.Set<ProductReservation>().AsNoTracking().Include(e => e.ProductInstance).Include(e => e.User);

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
            this._context.ProductReservations.Include(e => e.ProductInstance).Include(e => e.User).Load();
        }

        public async Task LoadDataAsync() {
            await this._context.ProductReservations.Include(e => e.ProductInstance).Include(e => e.User).LoadAsync();
        }
    }
}
