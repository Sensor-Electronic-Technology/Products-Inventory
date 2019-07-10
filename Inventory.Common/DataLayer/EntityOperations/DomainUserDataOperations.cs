using Inventory.Common.EntityLayer.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Inventory.Common.EntityLayer.Model;
using EntityFramework.Triggers;
using System.Linq.Expressions;
using Inventory.Common.ApplicationLayer;
using Inventory.Common.BuisnessLayer;

namespace Inventory.Common.DataLayer.EntityOperations {
    public class DomainUserDataOperations : IEntityDataOperations<User> {

        protected InventoryContext _context;
        protected IDomainManager _domainManager;
        protected IUserService _userService;

        public DomainUserDataOperations(InventoryContext context, IDomainManager domainManager, IUserService userService) {
            this._context = context;
            this._domainManager = domainManager;
            this._userService = userService;
        }

        public User Add(User entity) {
            var userEntity = this._context.Users.Include(e => e.Permission).FirstOrDefault(e => e.UserName == entity.UserName);

            if(userEntity == null) {
                var newPermission = this._context.Permissions.FirstOrDefault(e => e.Name == entity.Permission.Name);
                if(newPermission != null) {
                    entity.Permission = newPermission;
                    this._context.Users.Add(entity);
                    var userDomainPermissions = this._domainManager.AllUserInventoryPermsions(entity.UserName);
                    foreach(var item in userDomainPermissions) {
                        this._domainManager.RemoveUserFromGroup(entity.UserName, item);
                    }
                    if(this._domainManager.AddUserToGroup(entity.UserName, entity.Permission.Name)) {
                        return entity;
                    } else {
                        return null;
                    }
                } else {
                    return null;
                }
            } else {
                return this.Update(entity);
            }
        }

        public User Update(User entity) {
            var userEntity = this._context.Users.Include(e => e.Permission).FirstOrDefault(e => e.UserName == entity.UserName);
            if(userEntity != null) {
                var newPermission = this._context.Permissions.FirstOrDefault(e => e.Name == entity.Permission.Name);
                if(newPermission != null) {
                    if(newPermission.Name != userEntity.Permission.Name) {
                        userEntity.Permission = newPermission;

                        var userDomainPermissions = this._domainManager.AllUserInventoryPermsions(entity.UserName);
                        foreach(var item in userDomainPermissions) {
                            this._domainManager.RemoveUserFromGroup(userEntity.UserName, item);
                        }
                        if(this._domainManager.AddUserToGroup(entity.UserName, userEntity.Permission.Name)) {
                            this._context.Entry<User>(userEntity).State = EntityState.Modified;
                            return entity;
                        } else {
                            this._context.UndoDbEntry(userEntity);
                            return userEntity;
                        }
                    } else {
                        return null;
                    }
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        public User Delete(User entity) {
            var userEntity = this._context.Users
                .Include(e => e.Permission)
                .Include(e => e.Sessions)
                .Include(e => e.ProductReservations)
                .FirstOrDefault(e => e.UserName == entity.UserName);
            if(userEntity != null) {
                if(userEntity.Permission != null) {
                    var permission = this._context.Permissions
                        .Include(e => e.Users)
                        .FirstOrDefault(e => e.Name == userEntity.Permission.Name);
                    if(permission != null) {
                        permission.Users.Remove(userEntity);
                    }
                }
                var userDomainPermissions = this._domainManager.AllUserInventoryPermsions(entity.UserName);
                foreach(var item in userDomainPermissions) {
                    this._domainManager.RemoveUserFromGroup(userEntity.UserName, item);
                }
                userEntity.Sessions.ToList().ForEach(session => {
                    this._context.Sessions.Remove(session);
                });

                userEntity.ProductReservations.ToList().ForEach(reservation => {
                    this._context.ProductReservations.Remove(reservation);
                });
                return this._context.Users.Remove(userEntity);
            } else {
                var userDomainPermissions = this._domainManager.AllUserInventoryPermsions(entity.UserName);
                foreach(var item in userDomainPermissions) {
                    this._domainManager.RemoveUserFromGroup(userEntity.UserName, item);
                }
                return entity;
            }

        }

        public Tother AddToEntity<Tother>(Tother other) where Tother : class {
            return null;
        }

        public Tother RemoveFromEntity<Tother>(Tother other) where Tother : class {
            return null;
        }

        public void OnEntityDelete(IDeletingEntry<User, InventoryContext> entry) {
            var user = entry.Entity;
            Log log = new Log(this._userService.CurrentUser.UserName, user.GetType().ToString(), user.UserName, InventoryOperations.DELETE.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityInsert(IInsertingEntry<User, InventoryContext> entry) {
            var user = entry.Entity;
            Log log = new Log(this._userService.CurrentUser.UserName, user.GetType().ToString(), user.UserName, InventoryOperations.ADD.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityUpdate(IUpdatingEntry<User, InventoryContext> entry) {
            var user = entry.Entity;
            Log log = new Log(this._userService.CurrentUser.UserName, user.GetType().ToString(), user.UserName, InventoryOperations.UPDATE.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }
    }
}
