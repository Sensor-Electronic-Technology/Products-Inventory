namespace Inventory.Common.EntityLayer.Model.Entities {
    using EntityFramework.Triggers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public enum InventoryAction : int {
        OUTGOING,
        INCOMING,
        RETURNING
    }

    public abstract class Transaction : IEntityWithTracking {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public InventoryAction InventoryAction { get; set; }
        public bool IsReturning { get; set; }
        public byte[] RowVersion { get; set; }

        public int? OutgoingTransactionId { get; set; }
        public virtual Transaction OutgoingTransaction { get; set; }

        public int SessionId { get; set; }
        public virtual Session Session { get; set; }

        public int InstanceId { get; set; }
        public virtual Instance Instance { get; set; } //for ProductInstance will have transaction by rank
        public int Quantity { get; set; }

        public int? LocationId { get; set; }
        public virtual Location Location { get; set; }

        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        static Transaction()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }
    }

    public partial class ProductTransaction : Transaction {

        public string ProductName { get; set; }
        public string BuyerPoNumber { get; set; }    //Outgoing
        public string RMA_Number { get; set; }      //Return?
        public double? UnitCost { get; set; }
        public double? TotalCost { get; set; }

        public ProductTransaction()
        {

        }

        public ProductTransaction(ProductInstance instance,int quantity)
        {
            
            this.Quantity = quantity;
            this.Instance = instance;
            this.ProductName = instance.InventoryItem.Name;
        }
    }

    public partial class PartTransaction : Transaction {
        public double TrackedValue { get; set; }

        public int? InstanceParameterId { get; set; }
        public virtual InstanceParameter InstanceParameter { get; set; }

        public PartTransaction()
        {

        }
    }






}
