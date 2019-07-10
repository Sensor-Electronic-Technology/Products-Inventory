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
    public class ProductDataOperations : IEntityDataOperations<Product> {
        private InventoryContext _context;
        private IUserService _userService;

        public ProductDataOperations(InventoryContext context, IUserService userService) {
            this._context = context;
            this._userService = userService;
        }

        public Product Add(Product entity) {
            var product = this._context.InventoryItems.AsNoTracking().OfType<Product>().FirstOrDefault(e => e.Id == entity.Id);
            if(product == null) {
                product = this._context.InventoryItems.Create<Product>();
                var warehouse = this._context.Locations.AsNoTracking().OfType<Warehouse>().FirstOrDefault(x => x.Name == "Products");
                var category = this._context.Categories.AsNoTracking().OfType<ProductType>().FirstOrDefault(e => e.Id == entity.ProductTypeId);
                product.Name = entity.Name;
                product.CustomPartName = entity.CustomPartName;
                product.LegacyName = entity.LegacyName;
                product.Obsolete = entity.Obsolete;
                product.Description = entity.Description;

                if(category != null) {
                    product.ProductTypeId = category.Id;
                    category.Products.Add(product);
                }

                if(warehouse != null) {
                    product.WarehouseId = warehouse.Id;
                }
                this._context.InventoryItems.Add(product);
                try {
                    this._context.SaveChanges();
                    return entity;
                } catch {
                    this._context.UndoDbEntry(product);
                    return null;
                }
            } else {
                return null;
            }
        }

        public Product Update(Product entity) {
            var product = this._context.InventoryItems.OfType<Product>().FirstOrDefault(e => e.Id == entity.Id);
            if(product != null) {
                product.Name = entity.Name;
                product.CustomPartName = entity.CustomPartName;
                product.LegacyName = entity.LegacyName;
                product.Obsolete = entity.Obsolete;
                product.Description = entity.Description;
                var category=this._context.Categories.AsNoTracking().OfType<ProductType>().Include(e=>e.Products).FirstOrDefault(e => e.Id == entity.ProductTypeId);
                if(category != null) {
                    product.ProductTypeId = category.Id;
                }
                this._context.Entry<Product>(product).State = EntityState.Modified;
                try {
                    this._context.SaveChanges();
                    return entity;
                } catch {
                    this._context.UndoDbEntry(product);
                    return null;
                }
            } else {
                return null;
            }
        }

        public Product Delete(Product entity) {
            var product = this._context.InventoryItems.OfType<Product>()
                .Include(e => e.Attachments)
                .Include(e => e.Lots.Select(x => x.ProductInstances))
                .Include(e => e.Warehouse)
                .Include(e => e.ProductType)
                .Include(e => e.Organization)
                .Include(e => e.Manufacturers)
                .FirstOrDefault(e => e.Id == entity.Id);
            if(product != null) {
                product.Lots.ToList().ForEach(lot => {
                    var lotEntity = this._context.Lots.Include(e => e.ProductInstances.Select(i => i.Transactions))
                    .Include(e => e.Cost).FirstOrDefault(x => x.LotNumber == lot.LotNumber && x.SupplierPoNumber == lot.SupplierPoNumber);
                    lotEntity.ProductInstances.ToList().ForEach(rank => {
                        rank.Transactions.ToList().ForEach(t => {
                            this._context.Transactions.Remove(t);
                        });
                        this._context.Instances.Remove(rank);
                    });
                    lotEntity.ProductInstances.Clear();
                    this._context.Rates.Remove(lotEntity.Cost);
                    lotEntity.Cost = null;
                    lotEntity.Product = null;
                    this._context.Lots.Remove(lotEntity);
                });
                product.Lots.Clear();
                product.Instances.Clear();
                this._context.InventoryItems.Remove(product);
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

        public void OnEntityInsert(IInsertingEntry<Product, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.ADD.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityUpdate(IUpdatingEntry<Product, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.UPDATE.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityDelete(IDeletingEntry<Product, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), entry.Entity.Name, InventoryOperations.DELETE.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }
    }

    public class PartDataOperations : IEntityDataOperations<Part> {
        public Part Add(Part entity) => throw new NotImplementedException();
        public Part Delete(Part entity) => throw new NotImplementedException();
        public void OnEntityDelete(IDeletingEntry<Part, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityInsert(IInsertingEntry<Part, InventoryContext> entry) => throw new NotImplementedException();
        public void OnEntityUpdate(IUpdatingEntry<Part, InventoryContext> entry) => throw new NotImplementedException();
        public Part Update(Part entity) => throw new NotImplementedException();
    }
}
