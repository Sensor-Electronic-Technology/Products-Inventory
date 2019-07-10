using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EntityFramework.Triggers;
using Inventory.Common.BuisnessLayer;
using Inventory.Common.ApplicationLayer;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;

namespace Inventory.Common.DataLayer.EntityOperations {
    public class LotOperations : IEntityDataOperations<Lot> {

        private InventoryContext _context;
        private IUserService _userService;

        public LotOperations(InventoryContext context,IUserService userService) {
            this._context = context;
            this._userService = userService;
        }

        public Lot Add(Lot entity) {
            var lot = this._context.Lots.Find(entity.LotNumber, entity.SupplierPoNumber);
            if(lot == null) {
                this._context.Lots.Add(entity);
                return entity;
            }
            return null;
        }

        public Lot Update(Lot entity) {
            var lot = this._context.Lots
                .Include(e=>e.Cost)
                .Include(e=>e.Product)
                .Include(e=>e.ProductInstances)
                .FirstOrDefault(e=>e.LotNumber==entity.LotNumber && e.SupplierPoNumber==entity.SupplierPoNumber);
            if(lot != null) {
                if(lot.Cost != null) {
                    if(entity.Cost.Amount != lot.Cost.Amount) {
                        var cost = lot.Cost;
                        cost.Amount = entity.Cost.Amount;
                        this._context.Entry<Cost>(cost).State = EntityState.Modified;
                    }
                }
                lot.Recieved = entity.Recieved;
                this._context.Entry<Lot>(lot).State = EntityState.Modified;
                try {
                    this._context.SaveChanges();
                    return entity;
                } catch {
                    this._context.UndoDbContext();
                    return null;
                }
            } else {
                return null;
            }
        }

        public Lot Delete(Lot entity) => throw new NotImplementedException();

        public void OnEntityDelete(IDeletingEntry<Lot, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(),
                "(" + entry.Entity.LotNumber + "," + entry.Entity.SupplierPoNumber + ")",
                InventoryOperations.DELETE.GetDescription(), DateTime.UtcNow);
        }

        public void OnEntityInsert(IInsertingEntry<Lot, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), 
                "("+entry.Entity.LotNumber+","+entry.Entity.SupplierPoNumber+")", 
                InventoryOperations.ADD.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityUpdate(IUpdatingEntry<Lot, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(),
                "(" + entry.Entity.LotNumber + "," + entry.Entity.SupplierPoNumber + ")",
                InventoryOperations.UPDATE.GetDescription(), DateTime.UtcNow);
        }

    }
}
