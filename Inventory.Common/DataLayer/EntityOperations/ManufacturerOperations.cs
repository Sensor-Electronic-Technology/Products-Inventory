using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EntityFramework.Triggers;
using Inventory.Common.BuisnessLayer;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;

namespace Inventory.Common.DataLayer.EntityOperations {
    public class ManufacturerOperations : IEntityDataOperations<Manufacturer> {
        public Manufacturer Add(Manufacturer entity) => throw new NotImplementedException();
        public Task<Manufacturer> AddAsync(Manufacturer entity) => throw new NotImplementedException();
        public Tother AddToEntity<Tother>(Tother other) where Tother : class => throw new NotImplementedException();
        public Manufacturer Delete(Manufacturer entity) => throw new NotImplementedException();
        public Task<Manufacturer> DeleteAsync(Manufacturer entity) => throw new NotImplementedException();
        public void OnEntityDelete(IDeletingEntry<Manufacturer, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityInsert(IInsertingEntry<Manufacturer, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityUpdate(IUpdatingEntry<Manufacturer, InventoryContext> entry) => throw new NotImplementedException();
        public Tother RemoveFromEntity<Tother>(Tother other) where Tother : class => throw new NotImplementedException();
        public Manufacturer Update(Manufacturer entity) => throw new NotImplementedException();
        public Task<Manufacturer> UpdateAsync(Manufacturer entity) => throw new NotImplementedException();
    }
}
