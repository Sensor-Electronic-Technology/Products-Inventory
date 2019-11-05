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
    public class DistributorOperations : IEntityDataOperations<Distributor> {
        public Distributor Add(Distributor entity) => throw new NotImplementedException();
        public Task<Distributor> AddAsync(Distributor entity) => throw new NotImplementedException();
        public Distributor Delete(Distributor entity) => throw new NotImplementedException();
        public Task<Distributor> DeleteAsync(Distributor entity) => throw new NotImplementedException();
        public void OnEntityDelete(IDeletingEntry<Distributor, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityInsert(IInsertingEntry<Distributor, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityUpdate(IUpdatingEntry<Distributor, InventoryContext> entry) => throw new NotImplementedException();
        public Distributor Update(Distributor entity) => throw new NotImplementedException();
        public Task<Distributor> UpdateAsync(Distributor entity) => throw new NotImplementedException();
    }
}
