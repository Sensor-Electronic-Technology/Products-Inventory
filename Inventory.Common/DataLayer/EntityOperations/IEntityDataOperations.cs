using EntityFramework.Triggers;
using Inventory.Common.EntityLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Inventory.Common.DataLayer.EntityOperations {

    public interface IEntityDataOperations<TEntity> where TEntity:class {
        TEntity Add(TEntity entity);
        TEntity Delete(TEntity entity);
        TEntity Update(TEntity entity);

        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> DeleteAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);

        void OnEntityInsert(IInsertingEntry<TEntity, InventoryContext> entry);
        void OnEntityUpdate(IUpdatingEntry<TEntity, InventoryContext> entry);
        void OnEntityDelete(IDeletingEntry<TEntity, InventoryContext> entry);
    }
}
