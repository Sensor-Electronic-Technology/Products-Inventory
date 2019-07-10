using EntityFramework.Triggers;
using Inventory.Common.DataLayer.Providers;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Common.DataLayer.EntityDataManagers {
    public interface IEntityDataManager<TEntity> where TEntity : class {
        
        bool Save(TEntity entity);
        bool Delete(TEntity entity);
        void UndoAllChanges();
      
        void LoadData();
        Task LoadDataAsync();

        TEntity GetEntity(string entityName);
        TEntity GetEntity(Expression<Func<TEntity,bool>> expression);

        IEnumerable<TEntity> GetEntityList(Expression<Func<TEntity,bool>> expression=null,
            Func<IQueryable<TEntity>,IOrderedQueryable<TEntity>> orderBy=null);

        Task<IEnumerable<TEntity>> GetEntityListAsync(Expression<Func<TEntity, bool>> expression=null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        void OnEntityDelete(IDeletingEntry<TEntity, InventoryContext> entry);
        void OnEntityInsert(IInsertingEntry<TEntity, InventoryContext> entry);
        void OnEntityUpdate(IUpdatingEntry<TEntity, InventoryContext> entry);
    }
}
