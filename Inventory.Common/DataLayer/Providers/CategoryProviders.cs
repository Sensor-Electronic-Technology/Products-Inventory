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

    public class ProductTypeProvider : IEntityDataProvider<ProductType> {

        private InventoryContext _context;
        private IUserService _userService;

        public ProductTypeProvider(InventoryContext context, IUserService userService) {
            this._context = context;
            this._userService = userService;
        }

        public ProductType GetEntity(string entityName) {
            return this._context.Categories.OfType<ProductType>().AsNoTracking()
                .FirstOrDefault(e => e.Name == entityName);
        }

        public ProductType GetEntity(Expression<Func<ProductType, bool>> expression) {
            return this._context.Categories.OfType<ProductType>().AsNoTracking()
                .FirstOrDefault(expression);
        }

        public IEnumerable<ProductType> GetEntityList(Expression<Func<ProductType, bool>> expression = null, Func<IQueryable<ProductType>, IOrderedQueryable<ProductType>> orderBy = null) {
            IQueryable<ProductType> query = this._context.Set<ProductType>().AsNoTracking();

            if(expression != null) {
                query = query.Where(expression);
            }

            if(orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<ProductType>> GetEntityListAsync(Expression<Func<ProductType, bool>> expression = null,
            Func<IQueryable<ProductType>, IOrderedQueryable<ProductType>> orderBy = null) {

            IQueryable<ProductType> query = this._context.Set<ProductType>().AsNoTracking();

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
            this._context.Categories.Load();
        }

        public async Task LoadDataAsync() {
            await this._context.Categories.OfType<ProductType>().LoadAsync();
        }
    }

    public class OrganizationProvider : IEntityDataProvider<Organization> {
        public Organization GetEntity(string entityName) => throw new NotImplementedException();
        public Organization GetEntity(Expression<Func<Organization, bool>> expression) => throw new NotImplementedException();
        public IEnumerable<Organization> GetEntityList(Expression<Func<Organization, bool>> expression = null, Func<IQueryable<Organization>, IOrderedQueryable<Organization>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<Organization>> GetEntityListAsync(Expression<Func<Organization, bool>> expression = null, Func<IQueryable<Organization>, IOrderedQueryable<Organization>> orderBy = null) => throw new NotImplementedException();
        public void LoadData() => throw new NotImplementedException();
        public Task LoadDataAsync() => throw new NotImplementedException();
    }

    public class ConditionProvider : IEntityDataProvider<Condition> {
        public Condition GetEntity(string entityName) => throw new NotImplementedException();
        public Condition GetEntity(Expression<Func<Condition, bool>> expression) => throw new NotImplementedException();
        public IEnumerable<Condition> GetEntityList(Expression<Func<Condition, bool>> expression = null, Func<IQueryable<Condition>, IOrderedQueryable<Condition>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<Condition>> GetEntityListAsync(Expression<Func<Condition, bool>> expression = null, Func<IQueryable<Condition>, IOrderedQueryable<Condition>> orderBy = null) => throw new NotImplementedException();
        public void LoadData() => throw new NotImplementedException();
        public Task LoadDataAsync() => throw new NotImplementedException();
    }

    public class UsageProvider : IEntityDataProvider<Usage> {
        public Usage GetEntity(string entityName) => throw new NotImplementedException();
        public Usage GetEntity(Expression<Func<Usage, bool>> expression) => throw new NotImplementedException();
        public IEnumerable<Usage> GetEntityList(Expression<Func<Usage, bool>> expression = null, Func<IQueryable<Usage>, IOrderedQueryable<Usage>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<Usage>> GetEntityListAsync(Expression<Func<Usage, bool>> expression = null, Func<IQueryable<Usage>, IOrderedQueryable<Usage>> orderBy = null) => throw new NotImplementedException();
        public void LoadData() => throw new NotImplementedException();
        public Task LoadDataAsync() => throw new NotImplementedException();
    }

    public class PartTypeProvider : IEntityDataProvider<PartType> {
        public PartType GetEntity(string entityName) => throw new NotImplementedException();
        public PartType GetEntity(Expression<Func<PartType, bool>> expression) => throw new NotImplementedException();
        public IEnumerable<PartType> GetEntityList(Expression<Func<PartType, bool>> expression = null, Func<IQueryable<PartType>, IOrderedQueryable<PartType>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<PartType>> GetEntityListAsync(Expression<Func<PartType, bool>> expression = null, Func<IQueryable<PartType>, IOrderedQueryable<PartType>> orderBy = null) => throw new NotImplementedException();
        public void LoadData() => throw new NotImplementedException();
        public Task LoadDataAsync() => throw new NotImplementedException();
    }

}
