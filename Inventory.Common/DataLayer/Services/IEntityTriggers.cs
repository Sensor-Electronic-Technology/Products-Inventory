using EntityFramework.Triggers;
using System;
using System.Data.Entity;

namespace Inventory.Common.DataLayer.Services {

    public interface IEntityTriggers<TEntity,TContext> where TEntity:class where TContext:DbContext {

        void OnInsert(IInsertingEntry<TEntity,TContext> entry);
        void OnDelete(IDeletingEntry<TEntity, TContext> entry);
        void OnUpdate(IUpdatingEntry<TEntity, TContext> entry);
    }
}
