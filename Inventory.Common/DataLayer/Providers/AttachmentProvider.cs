using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EntityFramework.Triggers;
using Inventory.Common.ApplicationLayer;
using Inventory.Common.BuisnessLayer;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;

namespace Inventory.Common.DataLayer.Providers {
    public class AttachmentProvider : IEntityDataProvider<Attachment> {
        private InventoryContext _context;
        private IUserService _userService;

        public AttachmentProvider(InventoryContext context, IUserService userService) {
            this._context = context;
            this._userService = userService;
        }

        public Attachment GetEntity(string entityName) => throw new NotImplementedException();

        public Attachment GetEntity(Expression<Func<Attachment, bool>> expression) {
            return this._context.Attachments.AsNoTracking().FirstOrDefault(expression);
        }

        public IEnumerable<Attachment> GetEntityList(Expression<Func<Attachment, bool>> expression = null, Func<IQueryable<Attachment>, IOrderedQueryable<Attachment>> orderBy = null) {
            IQueryable<Attachment> query = this._context.Set<Attachment>().AsNoTracking();

            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<Attachment>> GetEntityListAsync(Expression<Func<Attachment, bool>> expression = null, Func<IQueryable<Attachment>, IOrderedQueryable<Attachment>> orderBy = null) {
            IQueryable<Attachment> query = this._context.Set<Attachment>().AsNoTracking();

            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return await orderBy(query).ToListAsync();
            } else {
                return await query.ToListAsync();
            }
        }

        public void LoadData() {
            this._context.Attachments.Load();
        }

        public async Task LoadDataAsync() {
            await this._context.Attachments.LoadAsync();
        }
    }
}
