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

namespace Inventory.Common.DataLayer.EntityOperations {

    public class PartInstanceOperations : IEntityDataOperations<PartInstance> {
        public PartInstance Add(PartInstance entity) => throw new NotImplementedException();
        public Task<PartInstance> AddAsync(PartInstance entity) => throw new NotImplementedException();
        public Tother AddToEntity<Tother>(Tother other) where Tother : class => throw new NotImplementedException();
        public PartInstance Delete(PartInstance entity) => throw new NotImplementedException();
        public Task<PartInstance> DeleteAsync(PartInstance entity) => throw new NotImplementedException();
        public void OnEntityDelete(IDeletingEntry<PartInstance, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityInsert(IInsertingEntry<PartInstance, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityUpdate(IUpdatingEntry<PartInstance, InventoryContext> entry) => throw new NotImplementedException();
        public Tother RemoveFromEntity<Tother>(Tother other) where Tother : class => throw new NotImplementedException();
        public PartInstance Update(PartInstance entity) => throw new NotImplementedException();
        public Task<PartInstance> UpdateAsync(PartInstance entity) => throw new NotImplementedException();
    }

    public class ProductInstanceOperations : IEntityDataOperations<ProductInstance> {
        private InventoryContext _context;
        private IUserService _userService;

        public ProductInstanceOperations(InventoryContext context, IUserService userService) {
            this._context = context;
            this._userService = userService;
        }

        public ProductInstance Add(ProductInstance entity) {
            return null;
        }

        public ProductInstance Delete(ProductInstance entity) => throw new NotImplementedException();

        public ProductInstance Update(ProductInstance entity) {
            var rank = this._context.Instances.OfType<ProductInstance>()
                .Include(e => e.CurrentLocation)
                .Include(e => e.Transactions)
                .Include(e => e.Lot)
                .Include(e => e.InventoryItem)
                .FirstOrDefault(e=>e.Id==entity.Id);
            if(rank != null) {
                rank.Name = entity.Name;
                rank.Quantity = entity.Quantity;
                rank.Wavelength = entity.Wavelength;
                rank.Power = entity.Power;
                rank.Voltage = entity.Voltage;
                rank.Obsolete = entity.Obsolete;
                rank.IsReserved = entity.IsReserved;
                this._context.Entry<ProductInstance>(rank).State = EntityState.Modified;
                try {
                    this._context.SaveChanges();
                    return entity;
                } catch {
                    this._context.UndoDbEntry(rank);
                    return null;
                }
            }
            return null;
        }

        public void OnEntityInsert(IInsertingEntry<ProductInstance, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.ADD.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityUpdate(IUpdatingEntry<ProductInstance, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.UPDATE.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityDelete(IDeletingEntry<ProductInstance, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.DELETE.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public Task<ProductInstance> AddAsync(ProductInstance entity) => throw new NotImplementedException();
        public Task<ProductInstance> DeleteAsync(ProductInstance entity) => throw new NotImplementedException();
        public Task<ProductInstance> UpdateAsync(ProductInstance entity) => throw new NotImplementedException();
    }
}
