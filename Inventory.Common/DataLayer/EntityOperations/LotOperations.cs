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
                .Include(e=>e.ProductInstances.Select(x=>x.Transactions))
                .FirstOrDefault(e=>e.LotNumber==entity.LotNumber && e.SupplierPoNumber==entity.SupplierPoNumber);
            if(lot != null) {
                if(lot.Cost != null) {
                    if(entity.Cost.Amount != lot.Cost.Amount) {
                        var cost = lot.Cost;
                        cost.Amount = entity.Cost.Amount;
                        this._context.Entry<Cost>(cost).State = EntityState.Modified;
                        lot.ProductInstances.ToList().ForEach(rank => {
                            rank.Transactions.ToList().ForEach(t => {
                                var transaction=this._context.Entry<ProductTransaction>((ProductTransaction)t);
                                transaction.Entity.UnitCost = cost.Amount;
                                transaction.Entity.TotalCost = transaction.Entity.Quantity * cost.Amount;
                                transaction.State = EntityState.Modified;
                            });
                        });
                    }
                }

                if(entity.Obsolete != lot.Obsolete) {
                    lot.Obsolete = entity.Obsolete;
                    lot.ProductInstances.ToList().ForEach(rank => {
                        rank.Obsolete = entity.Obsolete;
                        this._context.Entry<ProductInstance>(rank).State = EntityState.Modified;
                    });
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

        public Lot Delete(Lot entity) {
            this._context.Lots
                .Include(e => e.ProductInstances.Select(x => x.Transactions.Select(i => i.Session)))
                .Include(e => e.Cost)
                .Include(e => e.Product)
                .Load();
                var lot = this._context.Lots
                .AsNoTracking()
                .Include(e => e.ProductInstances.Select(i => i.Transactions))
                .Include(e => e.Cost).Include(e => e.Product)
                .FirstOrDefault(x => x.LotNumber == entity.LotNumber && x.SupplierPoNumber == entity.SupplierPoNumber);
            if (lot != null) {
                var lotEntity = this._context.Lots
                    .Include(e => e.ProductInstances.Select(i => i.Transactions))
                    .Include(e => e.Cost)
                    .Include(e => e.Product)
                    .FirstOrDefault(x => x.LotNumber == entity.LotNumber && x.SupplierPoNumber == entity.SupplierPoNumber);
                if (lotEntity != null) {
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
                    lot.ProductName = lot.Product.Name;
                    this._context.Lots.Remove(lotEntity);
                    try {
                        this._context.SaveChanges();
                        return lot;
                    } catch {
                        return null;
                    }
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        public async Task<Lot> DeleteAsync(Lot entity) {
            await this._context.Lots.
                Include(e => e.ProductInstances.Select(x => x.Transactions.Select(i => i.Session)))
                .Include(e => e.Cost)
                .Include(e => e.Product)
                .LoadAsync();
            var lot = await this._context.Lots
                .AsNoTracking()
                .Include(e => e.ProductInstances.Select(i => i.Transactions))
                .Include(e => e.Cost).Include(e => e.Product)
                .FirstOrDefaultAsync(x => x.LotNumber == entity.LotNumber && x.SupplierPoNumber == entity.SupplierPoNumber);
            if (lot != null) {
                var lotEntity = await this._context.Lots
                    .Include(e => e.ProductInstances.Select(i => i.Transactions))
                    .Include(e => e.Cost).Include(e => e.Product)
                    .FirstOrDefaultAsync(x => x.LotNumber == entity.LotNumber && x.SupplierPoNumber == entity.SupplierPoNumber);
                if (lotEntity != null) {
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
                    lot.ProductName = lot.Product.Name;
                    this._context.Lots.Remove(lotEntity);
                    try {
                        await this._context.SaveChangesAsync();
                        return lot;
                    } catch {
                        return null;
                    }
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }



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

        public Task<Lot> AddAsync(Lot entity) => throw new NotImplementedException();
        public Task<Lot> UpdateAsync(Lot entity) => throw new NotImplementedException();
    }
}
