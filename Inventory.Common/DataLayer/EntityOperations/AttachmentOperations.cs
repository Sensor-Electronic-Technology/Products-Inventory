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
    public class AttachmentOperations : IEntityDataOperations<Attachment> {
        public Attachment Add(Attachment entity) => throw new NotImplementedException();
        public Task<Attachment> AddAsync(Attachment entity) => throw new NotImplementedException();
        public Attachment Delete(Attachment entity) => throw new NotImplementedException();
        public Task<Attachment> DeleteAsync(Attachment entity) => throw new NotImplementedException();
        public void OnEntityDelete(IDeletingEntry<Attachment, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityInsert(IInsertingEntry<Attachment, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityUpdate(IUpdatingEntry<Attachment, InventoryContext> entry) => throw new NotImplementedException();
        public Attachment Update(Attachment entity) => throw new NotImplementedException();
        public Task<Attachment> UpdateAsync(Attachment entity) => throw new NotImplementedException();
    }
}
