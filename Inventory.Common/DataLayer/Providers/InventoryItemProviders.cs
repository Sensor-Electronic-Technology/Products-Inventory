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
    public class ProductDataProvider : IEntityDataProvider<Product> {
        private InventoryContext _context;
        private IUserService _userService;

        public ProductDataProvider(InventoryContext context, IUserService userService) {
            this._context = context;
            this._userService = userService;
        }

        public Product GetEntity(string entityName) {
            return this._context.InventoryItems.OfType<Product>()
            .AsNoTracking()
            .Include(e => e.Attachments)
            .Include(e => e.Lots.Select(x => x.ProductInstances))
            .Include(e => e.Warehouse)
            .Include(e => e.ProductType)
            .Include(e => e.Organization)
            .Include(e => e.Manufacturers)
            .FirstOrDefault(e => e.Name == entityName);
        }

        public Product GetEntity(Expression<Func<Product, bool>> expression) {
            return this._context.InventoryItems.OfType<Product>()
            .AsNoTracking()
            .Include(e => e.Attachments)
            .Include(e => e.Lots.Select(x => x.ProductInstances))
            .Include(e => e.Warehouse)
            .Include(e => e.ProductType)
            .Include(e => e.Organization)
            .Include(e => e.Manufacturers)
            .FirstOrDefault(expression);
        }

        public IEnumerable<Product> GetEntityList(Expression<Func<Product, bool>> expression = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null) {
            IQueryable<Product> query = this._context.Set<Product>().AsNoTracking()
             .Include(e => e.Attachments)
             .Include(e => e.Lots.Select(x => x.ProductInstances))
             .Include(e => e.Warehouse)
             .Include(e => e.ProductType)
             .Include(e => e.Organization)
             .Include(e => e.Manufacturers);

            if(expression != null) {
                query = query.Where(expression);
            }

            if(orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<Product>> GetEntityListAsync(Expression<Func<Product, bool>> expression = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null) {

            IQueryable<Product> query = this._context.Set<Product>().AsNoTracking()
             .Include(e => e.Attachments)
             .Include(e => e.Lots.Select(x => x.ProductInstances))
             .Include(e => e.Warehouse)
             .Include(e => e.ProductType)
             .Include(e => e.Organization)
             .Include(e => e.Manufacturers);

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
            this._context.InventoryItems.OfType<Product>()
             .Include(e => e.Attachments)
             .Include(e => e.Lots.Select(x => x.ProductInstances))
             .Include(e => e.Warehouse)
             .Include(e => e.ProductType)
             .Include(e => e.Organization)
             .Include(e => e.Manufacturers).Load();
        }

        public async Task LoadDataAsync() {
            await this._context.InventoryItems.OfType<Product>()
              .Include(e => e.Attachments)
              .Include(e => e.Lots.Select(x => x.ProductInstances))
              .Include(e => e.Warehouse)
              .Include(e => e.ProductType)
              .Include(e => e.Organization)
              .Include(e => e.Manufacturers).LoadAsync();
        }
    }

    public class PartDataProvider : IEntityDataProvider<Part> {
        public Part GetEntity(string entityName) => throw new NotImplementedException();
        public Part GetEntity(Expression<Func<Part, bool>> expression) => throw new NotImplementedException();
        public IEnumerable<Part> GetEntityList(Expression<Func<Part, bool>> expression = null, Func<IQueryable<Part>, IOrderedQueryable<Part>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<Part>> GetEntityListAsync(Expression<Func<Part, bool>> expression = null, Func<IQueryable<Part>, IOrderedQueryable<Part>> orderBy = null) => throw new NotImplementedException();
        public void LoadData() => throw new NotImplementedException();
        public Task LoadDataAsync() => throw new NotImplementedException();
    }
}
