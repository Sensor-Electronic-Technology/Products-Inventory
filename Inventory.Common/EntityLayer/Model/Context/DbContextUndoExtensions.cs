using EntityFramework.Triggers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;

namespace Inventory.Common.EntityLayer.Model {
    public static class DbContextUndoExtensions {

        public static void UndoDbContext(this DbContextWithTriggers context) {
            if(context == null) {
                throw new ArgumentNullException();
            }

            foreach(var entry in context.ChangeTracker.Entries()) {
                switch(entry.State) {

                    case EntityState.Added: {
                        entry.State = EntityState.Detached;
                        break;
                    }

                    case EntityState.Deleted: {
                        entry.Reload();
                        break;
                    }
                    case EntityState.Modified: {
                        entry.State = EntityState.Unchanged;
                        break;
                    }
                    default:break; 
                }
            }
        }

        public static void UndoDbEntries<T>(this DbContextWithTriggers context) where T : class {
            if(context == null) {
                throw new ArgumentNullException();
            }

            foreach(var entry in context.ChangeTracker.Entries<T>()) {
                switch(entry.State) {

                    case EntityState.Added: {
                        entry.State = EntityState.Detached;
                        break;
                    }

                    case EntityState.Deleted: {
                        entry.Reload();
                        break;
                    }
                    case EntityState.Modified: {
                        entry.State = EntityState.Unchanged;
                        break;
                    }
                    default: break;
                }
            }
        }

        public static void UndoDbEntry(this DbContextWithTriggers context,object entity) {
            if(context == null || entity==null) {
                throw new ArgumentNullException();
            }
            var entry = context.Entry(entity);
            switch(entry.State) {

                case EntityState.Added: {
                    entry.State = EntityState.Detached;
                    break;
                }

                case EntityState.Deleted: {
                    entry.Reload();
                    break;
                }

                case EntityState.Modified: {
                    entry.State = EntityState.Unchanged;
                    break;
                }

                default: break;
            }

        }

        public static void UndoDbEntityProperty(this DbContextWithTriggers context, object entity, string propertyName) {
            if(context == null || entity == null || propertyName == null) {
                throw new ArgumentException();
            }

            try {
                DbEntityEntry entry = context.Entry(entity);
                if(entry.State == EntityState.Added || entry.State == EntityState.Detached) {
                    return;
                }

                // Get and Set the Property value by the Property Name.
                object propertyValue = entry.OriginalValues.GetValue<object>(propertyName);
                entry.Property(propertyName).CurrentValue = entry.Property(propertyName).OriginalValue;
            } catch {
                throw;
            }
        }

        public static void UndoObjectContext(this ObjectContext context) {
            if(context == null) {
                throw new ArgumentException();
            }

            // If the states of the entities are Modified or Deleted, refresh the date from the database.
            IEnumerable<object> collection = from e in context.ObjectStateManager.GetObjectStateEntries(EntityState.Modified | EntityState.Deleted)
                                             select e.Entity;
            context.Refresh(RefreshMode.StoreWins, collection);

            // If the states of the entities are Added, detach these new entities.
            IEnumerable<object> AddedCollection = from e in context.ObjectStateManager.GetObjectStateEntries(EntityState.Added)
                                                  select e.Entity;
            foreach(object addedEntity in AddedCollection) {
                context.Detach(addedEntity);
            }
        }

        public static void UndoObjectEntities<T>(this ObjectContext context, ObjectSet<T> objectSets) where T : EntityObject {
            if(context == null || objectSets == null) {
                throw new ArgumentException();
            }

            IEnumerable<T> collection = from o in objectSets.AsEnumerable()
                                        where o.EntityState == EntityState.Modified ||
                                        o.EntityState == EntityState.Deleted
                                        select o;
            context.Refresh(RefreshMode.StoreWins, collection);

            IEnumerable<T> AddedCollection = (from e in context.ObjectStateManager.GetObjectStateEntries
                                                  (EntityState.Added)
                                              select e.Entity).ToList().OfType<T>();
            foreach(T entity in AddedCollection) {
                context.Detach(entity);
            }
        }

        public static void UndoObjectEntity(this ObjectContext context, EntityObject entity) {
            if(context == null || entity == null) {
                throw new ArgumentException();
            }

            if(entity.EntityState == EntityState.Modified || entity.EntityState == EntityState.Deleted) {
                context.Refresh(RefreshMode.StoreWins, entity);
            } else if(entity.EntityState == EntityState.Added) {
                context.Detach(entity);
            }
        }

        public static void UndoObjectEntityProperty
    (this ObjectContext context, EntityObject entity, string propertyName) {
            if(context == null || entity == null || propertyName == null) {
                throw new ArgumentException();
            }

            try {
                // Get the entry from the entity, so we can get the original values. And then we use the 
                // reflection to set the property value of the entity.
                ObjectStateEntry entry = context.ObjectStateManager.GetObjectStateEntry(entity);
                if(entry.State != EntityState.Added && entry.State != EntityState.Detached) {
                    object propertyValue = entry.OriginalValues[propertyName];
                    PropertyInfo propertyInfo = entity.GetType().GetProperty(propertyName);
                    propertyInfo.SetValue(entity, propertyValue, null);

                }
            } catch {
                throw;
            }
        }

    }
}
