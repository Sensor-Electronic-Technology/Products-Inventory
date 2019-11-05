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
    public class InventoryUserDataOperations : IEntityDataOperations<User> {
        protected InventoryContext _context;
        protected IDomainManager _domainManager;
        protected IUserService _userService;

        public InventoryUserDataOperations(InventoryContext context, IDomainManager domainManager, IUserService userService) {
            this._context = context;
            this._domainManager = domainManager;
            this._userService = userService;
        }

        public User Add(User entity) {
            if(entity != null) {
                var user = this._context.Users.FirstOrDefault(e => e.UserName == entity.UserName);
                if(user == null) {
                    if(entity.Permission != null) {
                        var permission = this._context.Permissions.FirstOrDefault(e => e.Name == entity.Permission.Name);
                        if(permission != null) {
                            entity.Permission = permission;
                            if(this._domainManager.AddUserToGroup(entity.UserName, entity.Permission.Name)) {
                                entity = this._context.Users.Add(entity);
                                this._context.Entry<User>(entity).State = EntityState.Detached;
                                return entity;
                            } else {
                                return null;
                            }
                        }
                    }
                } else {
                    return null;
                }
            }
            return null;
        }

        public Tother AddToEntity<Tother>(Tother other) where Tother : class {

            return null;
        }

        public Tother RemoveFromEntity<Tother>(Tother other) where Tother : class {
            return null;
        }

        public User Update(User entity) {
            var userEntity = this._context.Users.Include(e => e.Permission).FirstOrDefault(e => e.UserName == entity.UserName);
            if(userEntity != null) {
                var newPermission = this._context.Permissions.FirstOrDefault(e => e.Name == entity.Permission.Name);
                if(newPermission != null) {
                    if(userEntity.Permission.Name != newPermission.Name) {
                        userEntity.Permission = newPermission;
                        var userDomainPermissions = this._domainManager.AllUserInventoryPermsions(userEntity.UserName);
                        foreach(var item in userDomainPermissions) {
                            this._domainManager.RemoveUserFromGroup(userEntity.UserName, item);
                        }
                        if(this._domainManager.AddUserToGroup(userEntity.UserName, userEntity.Permission.Name)) {
                            this._context.Entry<User>(userEntity).State = EntityState.Modified;
                            return entity;
                        } else {
                            return null;
                        }
                    }
                }
            } else {
                return null;
            }
            return null;
        }

        public User Delete(User entity) {
            if(entity != null) {
                var user = this._context.Users
                .Include(e => e.Permission)
                .Include(e => e.Sessions)
                .FirstOrDefault(e => e.UserName == entity.UserName);
                if(user != null) {
                    if(this._domainManager.RemoveAllUserInventoryPermissions(user.UserName)) {
                        if(user.Permission != null) {
                            var permission = this._context.Permissions
                                .Include(e => e.Users)
                                .FirstOrDefault(e => e.Name == user.Permission.Name);
                            if(permission != null) {
                                var returnUser = permission.Users.Remove(user);
                            }
                        }
                        user.Permission = null;
                        return this._context.Users.Remove(user); ;
                    }
                } else {
                    return null;
                }
            }
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

        public Task<User> AddAsync(User entity) => throw new NotImplementedException();
        public Task<User> DeleteAsync(User entity) => throw new NotImplementedException();
        public Task<User> UpdateAsync(User entity) => throw new NotImplementedException();
    }


}
