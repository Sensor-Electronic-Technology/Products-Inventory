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
using Inventory.Common.BuisnessLayer;

namespace Inventory.Common.DataLayer.Providers {
    class InventoryUserProvider : IEntityDataProvider<User> {
        private InventoryContext _context;
        private IDomainManager _domainManager;
        private IUserService _userService;

        public InventoryUserProvider(InventoryContext context, IDomainManager domainManager, IUserService userService) {
            this._context = context;
            this._domainManager = domainManager;
            this._userService = userService;
        }

        public User GetEntity(Expression<Func<User, bool>> expression) {
            return this._context.Users
                .AsNoTracking()
                .Include(e => e.Permission)
                .Include(e => e.Sessions)
                .FirstOrDefault(expression);
        }

        public User GetEntity(string entityName) {
            return this._context.Users
                .AsNoTracking()
                .Include(e => e.Permission)
                .Include(e => e.Sessions)
                .FirstOrDefault(e => e.UserName == entityName);
        }

        public IEnumerable<User> GetEntityList(Expression<Func<User, bool>> expression = null,
            Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null) {
            IQueryable<User> usersQuery = this._context.Set<User>().AsNoTracking().Include(e => e.Permission);

            if(expression != null) {
                usersQuery = usersQuery.Where(expression);
            }

            if(orderBy != null) {
                return orderBy(usersQuery).ToList();
            } else {
                return usersQuery.ToList();
            }
        }

        public async Task<IEnumerable<User>> GetEntityListAsync(Expression<Func<User, bool>> expression,
            Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null) {

            IQueryable<User> usersQuery = this._context.Set<User>().Include(e => e.Permission).AsNoTracking();

            if(expression != null) {
                usersQuery = usersQuery.Where(expression);
            }

            if(orderBy != null) {
                return await orderBy(usersQuery).ToListAsync();
            } else {
                return await usersQuery.ToListAsync();
            }
        }

        public void LoadData() {
                this._context.Users.Include(e => e.Permission).Load();
                this._context.Permissions.Load();
        }

        public async Task LoadDataAsync() {
            var loadUsersTask = this._context.Users.Include(e => e.Permission).LoadAsync();
            var loadPermissionsTask = this._context.Permissions.LoadAsync();
            await Task.WhenAll(loadUsersTask, loadPermissionsTask);
        }
    }

    public class DomainUserProvider : IEntityDataProvider<User> {
        private InventoryContext _context;
        private IDomainManager _domainManager;
        private IUserService _userService;

        public DomainUserProvider(InventoryContext context, IDomainManager domainManager, IUserService userService) {
            this._context = context;
            this._domainManager = domainManager;
            this._userService = userService;
        }

        public User GetEntity(Expression<Func<User, bool>> expression) {
            return this._context.Users
                .AsNoTracking()
                .Include(e => e.Permission)
                .Include(e => e.Sessions)
                .FirstOrDefault(expression);
        }

        public User GetEntity(string entityName) {
            return this._domainManager.GetDomainUser(entityName);
        }

        public IEnumerable<User> GetEntityList(Expression<Func<User, bool>> expression = null, Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null) {
            return this._domainManager.GetSETUsers();
        }

        public async Task<IEnumerable<User>> GetEntityListAsync(Expression<Func<User, bool>> expression = null, Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null) {
            return await Task.Run(() => this._domainManager.GetSETUsers().ToList());
        }

        public void LoadData() {
            this._context.Users.Include(e => e.Permission).Load();
            this._context.Permissions.Load();
        }

        public async Task LoadDataAsync() {
            var loadUsersTask = this._context.Users.Include(e => e.Permission).LoadAsync();
            var loadPermissionsTask = this._context.Permissions.LoadAsync();
            await Task.WhenAll(loadUsersTask, loadPermissionsTask);
        }
    }
}
