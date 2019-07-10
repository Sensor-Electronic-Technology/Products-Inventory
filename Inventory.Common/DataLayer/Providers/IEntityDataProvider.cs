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

namespace Inventory.Common.DataLayer.Providers {
    public interface IEntityDataProvider<TEntity> where TEntity:class {
        void LoadData();
        Task LoadDataAsync();

        TEntity GetEntity(string entityName);
        TEntity GetEntity(Expression<Func<TEntity, bool>> expression);

        IEnumerable<TEntity> GetEntityList(Expression<Func<TEntity, bool>> expression = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        Task<IEnumerable<TEntity>> GetEntityListAsync(Expression<Func<TEntity, bool>> expression = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);
    }
}
