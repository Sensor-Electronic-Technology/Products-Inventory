namespace Inventory.Common.EntityLayer.Model.Entities {
    using EntityFramework.Triggers;
    using System;
    using System.Collections.Generic;

    public abstract class Instance: IEntityWithTracking {
        public int Id { get; set; }
        public string Name { get; set; }//Rank}
        public string SkuNumber { get; set; }
        public int Quantity { get; set; }
        public int MinQuantity { get; set; }
        public int SafeQuantity { get; set; }
        public byte[] RowVersion { get; set; }

        public int InventoryItemId { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }

        public int? CurrentLocationId { get; set; }
        public virtual Location CurrentLocation { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        //Tracking
        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        static Instance()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public Instance()
        {
            this.Transactions = new HashSet<Transaction>();
        }
    }

    public partial class ProductInstance:Instance {
        //Composite Key
        public string LotNumber { get; set; }
        public string SupplierPoNumber { get; set; }
        public virtual Lot Lot { get; set; }
        public bool IsReserved { get; set; }
        public bool Obsolete { get; set; }
        //Ranks
        public string Wavelength { get; set; }
        public string Power { get; set; }
        public string Voltage { get; set; }

        public virtual ICollection<ProductReservation> ProductReservations { get; set; }

        static ProductInstance()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public ProductInstance()
        {
            this.Transactions = new HashSet<Transaction>();
            this.ProductReservations = new HashSet<ProductReservation>();
            this.Obsolete = false;
            this.IsReserved = false;
        }

        public ProductInstance(Lot lot) : this()
        {
            this.Lot = lot;
        }
    }

    public partial class PartInstance : Instance {
        public string SerialNumber { get; set; }
        public string BatchNumber { get; set; }
        public bool IsResuable { get; set; }

        //public int PartId { get; set; }
        //public virtual Part Part { get; set; }

        public int? PartTypeId { get; set; }
        public virtual PartType PartType { get; set; }

        public int? ConditionId { get; set; }
        public virtual Condition Condition { get; set; }

        public virtual ICollection<Price> Prices { get; set; }
        public virtual ICollection<InstanceParameter> InstanceParameters { get; set; }

        static PartInstance()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public PartInstance()
        {
            this.InstanceParameters = new HashSet<InstanceParameter>();
            this.Transactions = new HashSet<Transaction>();
            this.Prices = new HashSet<Price>();
        }

        public PartInstance(Part part,string name,string serialNumber, string batchNumber, string skuNumber):this()
        {
            this.Name = name;
            this.SerialNumber = serialNumber;
            this.BatchNumber = batchNumber;
            this.SkuNumber = skuNumber;
            //this.Part = part;
        }
    }
}
