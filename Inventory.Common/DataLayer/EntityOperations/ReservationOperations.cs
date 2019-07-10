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
    public class ReservationOperations:IEntityDataOperations<ProductReservation> {
        private InventoryContext _context;
        private IUserService _userService;

        public ReservationOperations(InventoryContext context, IUserService userService) {
            this._context = context;
            this._userService = userService;
            Triggers<ProductReservation, InventoryContext>.Updating += this.OnEntityUpdate;
            Triggers<ProductReservation, InventoryContext>.Inserting += this.OnEntityInsert;
            Triggers<ProductReservation, InventoryContext>.Deleting += this.OnEntityDelete;
        }

        public ProductReservation Add(ProductReservation entity) {
            if(entity != null) {
                var rank = this._context.Instances.OfType<ProductInstance>().Include(e => e.Lot).Include(e=>e.ProductReservations).Include(e=>e.InventoryItem).FirstOrDefault(e => e.Id == entity.ProductInstance.Id);
                if(rank!=null) {
                    var reservation = this._context.ProductReservations.Create();
                    reservation.Quantity = entity.Quantity;
                    reservation.TimeStamp = entity.TimeStamp;
                    reservation.Expiration = entity.Expiration;
                    reservation.RMANumber = entity.RMANumber;
                    reservation.BuyerPoNumber = entity.BuyerPoNumber;
                    reservation.Customer = entity.Customer;
                    reservation.Note = entity.Note;
                    reservation.ProductInstancdId = rank.Id;
                    reservation.LotNumber = rank.LotNumber;
                    reservation.PoNumber = rank.SupplierPoNumber;
                    reservation.UserId = this._userService.CurrentUser.Id;
                    reservation.ProductName = rank.InventoryItem.Name;
                    reservation.Rank = rank.Name;
                    this._context.ProductReservations.Add(reservation);
                    rank.IsReserved = true;
                    rank.ProductReservations.Add(reservation);
                    this._context.Entry<ProductInstance>(rank).State = EntityState.Modified;
                    try {
                        this._context.SaveChanges();
                        return entity;
                    } catch {
                        this._context.UndoDbEntry(reservation);
                        this._context.UndoDbEntry(rank);
                        return null;
                    }
                } else {
                    return null;
                }
            }
            return null;
        }

        public ProductReservation Update(ProductReservation entity) {
            if(entity != null) {
                var reservation = this._context.ProductReservations
                    .Include(e=>e.ProductInstance.Lot)
                    .FirstOrDefault(e=>e.Id==entity.Id);
                if(reservation != null ) {
                    reservation.Quantity = entity.Quantity;
                    reservation.TimeStamp = entity.TimeStamp;
                    reservation.Expiration = entity.Expiration;
                    reservation.RMANumber = entity.RMANumber;
                    reservation.BuyerPoNumber = entity.BuyerPoNumber;
                    reservation.Customer = entity.Customer;
                    reservation.Note = entity.Note;
                    this._context.Entry<ProductReservation>(reservation).State = EntityState.Modified;
                    try {
                        this._context.SaveChanges();
                        return entity;
                    } catch {
                        this._context.UndoDbEntry(reservation);
                        return null;
                    }
                } else {
                    return null;
                }
            }
            return null;
        }

        public ProductReservation Delete(ProductReservation entity) {
            if(entity != null) {
                var reservation = this._context.ProductReservations
                    .Include(e => e.ProductInstance.Lot)
                    .Include(e=>e.User)
                    .FirstOrDefault(e => e.Id == entity.Id);
                if(reservation != null) {
                    var rank = this._context.Instances.OfType<ProductInstance>().Include(e => e.ProductReservations).FirstOrDefault(e=>e.Id==reservation.ProductInstancdId);
                    if(rank != null) {
                        rank.ProductReservations.Remove(reservation);
                        if(rank.ProductReservations.Count == 0) {
                            rank.IsReserved = false;
                        }
                        this._context.Entry<ProductInstance>(rank).State = EntityState.Modified;
                        this._context.ProductReservations.Remove(reservation);
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
            }
            return null;
        }

        public void OnEntityInsert(IInsertingEntry<ProductReservation, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), "Id: "+entry.Entity.Id.ToString()+" Rank: "+entry.Entity.Rank+" Product: "+entry.Entity.ProductName, InventoryOperations.ADD.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityUpdate(IUpdatingEntry<ProductReservation, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), "Id: " + entry.Entity.Id.ToString() + " Rank: " + entry.Entity.Rank + " Product: " + entry.Entity.ProductName, InventoryOperations.UPDATE.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }

        public void OnEntityDelete(IDeletingEntry<ProductReservation, InventoryContext> entry) {
            Log log = new Log(this._userService.CurrentUser.UserName, entry.Entity.GetType().ToString(), "Id: " + entry.Entity.Id.ToString() + " Rank: " + entry.Entity.Rank + " Product: " + entry.Entity.ProductName, InventoryOperations.DELETE.GetDescription(), DateTime.UtcNow);
            log.SessionId = this._userService.CurrentSession.Id;
            entry.Context.Logs.Add(log);
        }
    }
}
