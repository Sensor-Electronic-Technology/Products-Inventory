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
    public class ProductTypeOperations : IEntityDataOperations<ProductType> {
        private InventoryContext _context;
        private IUserService _userService;

        public ProductTypeOperations(InventoryContext context, IUserService userService) {
            this._context = context;
            this._userService = userService;
            Triggers<ProductType, InventoryContext>.Updating += this.OnEntityUpdate;
            Triggers<ProductType, InventoryContext>.Deleting += this.OnEntityDelete;
            Triggers<ProductType, InventoryContext>.Inserting += this.OnEntityInsert;
        }

        public ProductType Add(ProductType entity) {
            var pType = this._context.Categories.OfType<ProductType>().FirstOrDefault(x => x.Name == entity.Name);
            if(pType == null) {
                pType = new ProductType();
                pType.Name = entity.Name;
                pType.Description = entity.Description;
                return (ProductType)this._context.Categories.Add(pType);
            }
            return null;
        }

        public ProductType Delete(ProductType entity) {
            var pType = this._context.Categories.OfType<ProductType>()
                .Include(e=>e.Products)
                .FirstOrDefault(x => x.Id == entity.Id);
            if(pType != null) {
                pType.Products.ToList().ForEach(p => {
                    p.ProductType = null;
                });
                pType.Products.Clear();
                return (ProductType)this._context.Categories.Remove(pType);
            }
            return null;
        }

        public ProductType Update(ProductType entity) {
            var pType = this._context.Categories.OfType<ProductType>().FirstOrDefault(x => x.Id==entity.Id);
            if(pType != null) {
                pType.Name = entity.Name;
                pType.Description = entity.Description;
                this._context.Entry<ProductType>(pType).State = EntityState.Modified;
                return entity;
            }
            return null;
        }

        public void OnEntityInsert(IInsertingEntry<ProductType, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.ADD.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityUpdate(IUpdatingEntry<ProductType, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.UPDATE.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityDelete(IDeletingEntry<ProductType, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.DELETE.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }
    }

    public class OrganizationOperations : IEntityDataOperations<Organization> {
        public Organization Add(Organization entity) => throw new NotImplementedException();
        public Organization Delete(Organization entity) => throw new NotImplementedException();
        public void OnEntityDelete(IDeletingEntry<Organization, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityInsert(IInsertingEntry<Organization, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityUpdate(IUpdatingEntry<Organization, InventoryContext> entry) => throw new NotImplementedException();
        public Organization Update(Organization entity) => throw new NotImplementedException();
    }

    public class ConditionOperations : IEntityDataOperations<Condition> {
        public Condition Add(Condition entity) => throw new NotImplementedException();
        public Condition Delete(Condition entity) => throw new NotImplementedException();
        public void OnEntityDelete(IDeletingEntry<Condition, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityInsert(IInsertingEntry<Condition, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityUpdate(IUpdatingEntry<Condition, InventoryContext> entry) => throw new NotImplementedException();
        public Condition Update(Condition entity) => throw new NotImplementedException();
    }

    public class UsageOperations : IEntityDataOperations<Usage> {
        public Usage Add(Usage entity) => throw new NotImplementedException();
        public Usage Delete(Usage entity) => throw new NotImplementedException();
        public void OnEntityDelete(IDeletingEntry<Usage, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityInsert(IInsertingEntry<Usage, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityUpdate(IUpdatingEntry<Usage, InventoryContext> entry) => throw new NotImplementedException();
        public Usage Update(Usage entity) => throw new NotImplementedException();
    }

    public class PartTypeOperations : IEntityDataOperations<PartType> {
        public PartType Add(PartType entity) => throw new NotImplementedException();
        public PartType Delete(PartType entity) => throw new NotImplementedException();
        public void OnEntityDelete(IDeletingEntry<PartType, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityInsert(IInsertingEntry<PartType, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityUpdate(IUpdatingEntry<PartType, InventoryContext> entry) => throw new NotImplementedException();
        public PartType Update(PartType entity) => throw new NotImplementedException();
    }
}
