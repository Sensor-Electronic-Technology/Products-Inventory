using EntityFramework.Triggers;
using Inventory.Common.BuisnessLayer;
using Inventory.Common.ApplicationLayer;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Data.Entity.Infrastructure;

namespace Inventory.Common.DataLayer.EntityOperations {

    public class LocationOperations : IEntityDataOperations<Location> {

        private InventoryContext _context;
        private IUserService _userService;

        public LocationOperations(InventoryContext context,IUserService userService) {
            this._context = context;
            this._userService = userService;
            Triggers<Location, InventoryContext>.Updating += this.OnEntityUpdate;
            Triggers<Location, InventoryContext>.Deleting += this.OnEntityDelete;
            Triggers<Location, InventoryContext>.Inserting += this.OnEntityInsert;
        }

        public Location Add(Location entity) {
            var location = this._context.Locations.FirstOrDefault(e => e.Name == entity.Name);
            if(location == null) {
                this._context.Locations.Add(entity);
                bool savedFailed = false;
                int failCount = 0;
                do {
                    savedFailed = false;
                    try {
                        this._context.SaveChanges();
                    } catch(DbUpdateConcurrencyException e) {
                        failCount += 1;
                        savedFailed = true;
                        var entry = e.Entries.Single();
                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    }
                } while(savedFailed && failCount < 4);

                return savedFailed ? null : entity;
            } else {
                return null;
            }
        }

        public Location Delete(Location entity) {
            var location = this._context.Locations.FirstOrDefault(e => e.Id == entity.Id);
            if(location != null) {
                var retLocation = this._context.Locations.Remove(location);
                return retLocation;
            } else {
                return null;
            }
        }

        public Location Update(Location entity) {
            var location = this._context.Locations.FirstOrDefault(e => e.Id == entity.Id);
            if(location != null) {
                location.Name = entity.Name;
                location.Description = entity.Description;
                this._context.Entry<Location>(location).State = EntityState.Modified;
                bool savedFailed = false;
                int failCount = 0;
                do {
                    savedFailed = false;
                    try {
                        this._context.SaveChanges();
                    } catch(DbUpdateConcurrencyException e) {
                        failCount+=1;
                        savedFailed = true;
                        var entry = e.Entries.Single();
                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    }
                } while(savedFailed && failCount < 4);

                return savedFailed ? null:entity;
            } else {
                return null;
            }
        }

        public void OnEntityInsert(IInsertingEntry<Location, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.ADD.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityUpdate(IUpdatingEntry<Location, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.UPDATE.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityDelete(IDeletingEntry<Location, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.DELETE.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }
    }



    public class WarehouseOperations : IEntityDataOperations<Warehouse> {

        private InventoryContext _context;
        private IUserService _userService;

        public WarehouseOperations(InventoryContext context, IUserService userService) {
            this._context = context;
            this._userService = userService;
            Triggers<Warehouse, InventoryContext>.Updating += this.OnEntityUpdate;
            Triggers<Warehouse, InventoryContext>.Deleting += this.OnEntityDelete;
            Triggers<Warehouse, InventoryContext>.Inserting += this.OnEntityInsert;
        }

        public Warehouse Add(Warehouse entity) {
            var location = this._context.Locations.OfType<Warehouse>()
                .Include(e => e.ItemsAtLocation)
                .Include(e => e.StoredItems)
                .Include(e => e.Transactions)
                .FirstOrDefault(e => e.Name == entity.Name);
            if(location == null) {
                this._context.Locations.Add(entity);
                return entity;
            } else {
                return null;
            }
        }

        public Warehouse Delete(Warehouse entity) {
            var location = this._context.Locations.OfType<Warehouse>()
                .Include(e => e.ItemsAtLocation)
                .Include(e => e.StoredItems)
                .Include(e => e.Transactions)
                .FirstOrDefault(e => e.Id == entity.Id);
            if(location != null) {
                var retLocation = this._context.Locations.Remove(location) as Warehouse;
                return retLocation;
            } else {
                return null;
            }
        }
        public Warehouse Update(Warehouse entity) {
            var location = this._context.Locations.OfType<Warehouse>()
                .Include(e => e.ItemsAtLocation)
                .Include(e => e.StoredItems)
                .Include(e => e.Transactions)
                .FirstOrDefault(e => e.Id == entity.Id);
            if(location != null) {
                location.Name = entity.Name;
                location.Description = entity.Description;
                this._context.Entry<Location>(location).State = EntityState.Modified;
                return entity;
            } else {
                return null;
            }
        }

        public void OnEntityInsert(IInsertingEntry<Warehouse, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.ADD.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityUpdate(IUpdatingEntry<Warehouse, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.UPDATE.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityDelete(IDeletingEntry<Warehouse, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.DELETE.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }
    }

    public class ConsumerOperations : IEntityDataOperations<Consumer> {
        public static readonly Type PartInstanceType = typeof(PartInstance);
        public static readonly Type InstanceType = typeof(Instance);
        public static readonly Type InventoryItemType = typeof(InventoryItem);

        private InventoryContext _context;
        private IUserService _userService;

        public ConsumerOperations(InventoryContext context, IUserService userService) {
            this._context = context;
            this._userService = userService;
            Triggers<Consumer, InventoryContext>.Updating += this.OnEntityUpdate;
            Triggers<Consumer, InventoryContext>.Deleting += this.OnEntityDelete;
            Triggers<Consumer, InventoryContext>.Inserting += this.OnEntityInsert;
        }

        public Consumer Add(Consumer entity) {
            var location = this._context.Locations.OfType<Consumer>()
                .Include(e => e.ItemsAtLocation)
                .Include(e => e.Transactions)
                .FirstOrDefault(e => e.Name == entity.Name);
            if(location == null) {
                var consumer=this._context.Locations.Create<Consumer>();
                consumer.Name = entity.Name;
                consumer.Description = entity.Description;
                this._context.Locations.Add(consumer);
                return entity;
            } else {
                return null;
            }
        }

        public Consumer Delete(Consumer entity) {
            var location = this._context.Locations.OfType<Consumer>()
                .Include(e => e.ItemsAtLocation)
                .Include(e => e.Transactions)
                .FirstOrDefault(e => e.Id == entity.Id);
            if(location != null) {
                var retLocation = this._context.Locations.Remove(location) as Consumer;
                return retLocation;
            } else {
                return null;
            }
        }

        public Consumer Update(Consumer entity) {
            var location = this._context.Locations.OfType<Consumer>()
                .Include(e => e.ItemsAtLocation)
                .Include(e => e.Transactions)
                .FirstOrDefault(e => e.Id == entity.Id);
            if(location != null) {
                location.Name = entity.Name;
                location.Description = entity.Description;
                this._context.Entry<Location>(location).State = EntityState.Modified;
                return entity;
            } else {
                return null;
            }
        }

        public void OnEntityInsert(IInsertingEntry<Consumer, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.ADD.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityUpdate(IUpdatingEntry<Consumer, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.UPDATE.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityDelete(IDeletingEntry<Consumer, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.DELETE.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }
    }
}
