namespace Inventory.Common.EntityLayer.Model.Entities {
    using EntityFramework.Triggers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ProductReservation : IEntityWithTracking {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime? Expiration { get; set; }
        public int Quantity { get; set; }
        public string RMANumber { get; set; }
        public string BuyerPoNumber { get; set; }
        public string Rank { get; set; }
        public string Customer { get; set; }
        public string Note { get; set; }

        public string LotNumber { get; set; }
        public string PoNumber { get; set; }
        public string ProductName { get; set; }

        public int ProductInstancdId { get; set; }
        public virtual ProductInstance ProductInstance { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public byte[] RowVersion { get; set; }

        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        static ProductReservation()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public ProductReservation() {
            this.TimeStamp = DateTime.Now;
            this.Rank = "";
            this.ProductName = "";
            this.LotNumber = "";
            this.PoNumber = "";
            this.Quantity = 0;
            this.Customer = "";
            this.BuyerPoNumber = "";
            this.RMANumber = "";
            this.Note = "";
        }

        public ProductReservation(ProductInstance rank) {
            this.TimeStamp = DateTime.Now;
            this.ProductInstance = rank;
            this.Rank = rank.Name;
            this.ProductName = (rank.InventoryItem != null) ? rank.InventoryItem.Name : "Error";
            this.LotNumber = rank.LotNumber;
            this.PoNumber = rank.SupplierPoNumber;
            this.Quantity = 0;
            this.Customer = "";
            this.BuyerPoNumber = "";
            this.RMANumber = "";
            this.Note = "";
        }

        public ProductReservation(ProductInstance rank, int quantity) {
            this.ProductInstance = rank;
            this.Rank = rank.Name;
            this.ProductName = (rank.InventoryItem != null) ? rank.InventoryItem.Name : "Error";
            this.LotNumber = rank.LotNumber;
            this.PoNumber = rank.SupplierPoNumber;
            this.Quantity = quantity;
            this.TimeStamp = DateTime.Now;
        }

        public ProductReservation(ProductInstance rank, DateTime expiration, int quantity, string customer, string buyerPo, string rma,string note) {
            this.ProductInstance = rank;
            this.Rank = rank.Name;
            this.ProductName = (rank.InventoryItem != null) ? rank.InventoryItem.Name : "Error";
            this.LotNumber = rank.LotNumber;
            this.PoNumber = rank.SupplierPoNumber;
            this.Quantity = quantity;
            this.Customer = customer;
            this.BuyerPoNumber = buyerPo;
            this.RMANumber = rma;
            this.Expiration = expiration;
            this.TimeStamp = DateTime.Now;
            this.Note = note;
        }
    }
}
