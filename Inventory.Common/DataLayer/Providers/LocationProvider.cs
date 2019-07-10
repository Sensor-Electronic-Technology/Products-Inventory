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
    public class LocationProvider : IEntityDataProvider<Location> {
        private InventoryContext _context;
        private IUserService _userService;

        public LocationProvider(InventoryContext context,IUserService userService) {
            this._context = context;
            this._userService = userService;
        }

        public Location GetEntity(string entityName) {
            return this._context.Locations.AsNoTracking().FirstOrDefault(x => x.Name == entityName);
        }

        public Location GetEntity(Expression<Func<Location, bool>> expression) {
            return this._context.Locations.AsNoTracking().FirstOrDefault(expression);
        }

        public IEnumerable<Location> GetEntityList(Expression<Func<Location, bool>> expression = null,
            Func<IQueryable<Location>, IOrderedQueryable<Location>> orderBy = null) {

            IQueryable<Location> query = this._context.Set<Location>().AsNoTracking();

            if(expression != null) {
                query = query.Where(expression);
            }

            if(orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<Location>> GetEntityListAsync(Expression<Func<Location, bool>> expression = null,
            Func<IQueryable<Location>, IOrderedQueryable<Location>> orderBy = null) {
            IQueryable<Location> query = this._context.Set<Location>().AsNoTracking();

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
            this._context.Locations.Load();
        }

        public async Task LoadDataAsync() {
            await this._context.Locations.LoadAsync();
        }
    }

    public class WarehouseProvider : IEntityDataProvider<Warehouse> {
        private InventoryContext _context;
        private IUserService _userService;

        public WarehouseProvider(InventoryContext context, IUserService userService) {
            this._context = context;
            this._userService = userService;
        }

        public Warehouse GetEntity(string entityName) {
            return this._context.Locations.OfType<Warehouse>()
                .AsNoTracking()
                .Include(x=>x.ItemsAtLocation)
                .Include(x=>x.StoredItems)
                .Include(e=>e.Transactions)
                .FirstOrDefault(x=>x.Name==entityName);
        }
        public Warehouse GetEntity(Expression<Func<Warehouse, bool>> expression) {
            return this._context.Locations.OfType<Warehouse>()
                .AsNoTracking()
                .Include(x => x.ItemsAtLocation)
                .Include(x => x.StoredItems)
                .Include(e => e.Transactions)
                .FirstOrDefault(expression);
        }
        public IEnumerable<Warehouse> GetEntityList(Expression<Func<Warehouse, bool>> expression = null,
            Func<IQueryable<Warehouse>, IOrderedQueryable<Warehouse>> orderBy = null) {

            IQueryable<Warehouse> query = this._context.Set<Warehouse>()
                .AsNoTracking()
                .Include(x => x.ItemsAtLocation)
                .Include(x => x.StoredItems)
                .Include(e => e.Transactions);

            if(expression != null) {
                query = query.Where(expression);
            }

            if(orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<Warehouse>> GetEntityListAsync(Expression<Func<Warehouse, bool>> expression = null, 
            Func<IQueryable<Warehouse>, IOrderedQueryable<Warehouse>> orderBy = null) {
            IQueryable<Warehouse> query = this._context.Set<Warehouse>()
                .AsNoTracking()
                .Include(x => x.ItemsAtLocation)
                .Include(x => x.StoredItems)
                .Include(e => e.Transactions);
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
            this._context.Locations.OfType<Consumer>()
                .Load();
        }

        public async Task LoadDataAsync() {
            await this._context.Locations.OfType<Warehouse>()
                .LoadAsync();
        }
    }

    public class ConsumerProvider : IEntityDataProvider<Consumer> {
        private InventoryContext _context;
        private IUserService _userService;

        public ConsumerProvider(InventoryContext context, IUserService userService) {
            this._context = context;
            this._userService = userService;
        }

        public Consumer GetEntity(string entityName) {
            return this._context.Locations.OfType<Consumer>()
                .AsNoTracking()
                .FirstOrDefault(x => x.Name == entityName);
        }

        public Consumer GetEntity(Expression<Func<Consumer, bool>> expression) {
            return this._context.Locations.OfType<Consumer>()
                .AsNoTracking()
                .FirstOrDefault(expression);
        }

        public IEnumerable<Consumer> GetEntityList(Expression<Func<Consumer, bool>> expression = null,
            Func<IQueryable<Consumer>, IOrderedQueryable<Consumer>> orderBy = null) {

            IQueryable<Consumer> query = this._context.Set<Consumer>()
                .AsNoTracking();

            if(expression != null) {
                query = query.Where(expression);
            }

            if(orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<Consumer>> GetEntityListAsync(Expression<Func<Consumer, bool>> expression = null,
            Func<IQueryable<Consumer>, IOrderedQueryable<Consumer>> orderBy = null) {
            IQueryable<Consumer> query = this._context.Set<Consumer>()
                .AsNoTracking();

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
            this._context.Locations.OfType<Consumer>()
                .Include(e => e.Transactions)
                .Load();
        }

        public async Task LoadDataAsync() {
            await this._context.Locations.OfType<Consumer>()
                .LoadAsync();
        }
    }
}
