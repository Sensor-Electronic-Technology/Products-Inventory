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
using Inventory.Common.DataLayer.Providers;
using Inventory.Common.DataLayer.EntityOperations;
using Inventory.Common.BuisnessLayer;

namespace Inventory.Common.DataLayer.EntityOperations {

    public class CostOperations : IEntityDataOperations<Cost> {

        private InventoryContext _context;
        private IUserService _userService;

        public CostOperations(InventoryContext context, IUserService userService) {
            this._context = context;
            this._userService = userService;
        }

        public Cost Add(Cost entity) {
            this._context.Rates.Add(entity);
            return entity;
        }

        public Cost Update(Cost entity) {
            var cost=this._context.Rates.OfType<Cost>().FirstOrDefault(e => e.Id == entity.Id);
            if(cost != null) {
                cost.Amount = entity.Amount;
                cost.Distributor = entity.Distributor;
                //TODO:  Attachments
                this._context.Entry<Cost>(cost).State = EntityState.Modified;
                return entity;
            } else {
                return null;
            }
        }


        public Cost Delete(Cost entity) => throw new NotImplementedException();
        public void OnEntityDelete(IDeletingEntry<Cost, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityInsert(IInsertingEntry<Cost, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityUpdate(IUpdatingEntry<Cost, InventoryContext> entry) => throw new NotImplementedException();
        public Tother RemoveFromEntity<Tother>(Tother other) where Tother : class => throw new NotImplementedException();
        public Tother AddToEntity<Tother>(Tother other) where Tother : class => throw new NotImplementedException();
    }
}
