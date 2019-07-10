namespace Inventory.Common.EntityLayer.Model.Entities {
    using EntityFramework.Triggers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class Rate {
        public int Id { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public DateTime? VaildFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public double Amount { get; set; }
        public int MinOrder { get; set; }
        public double LeadTime { get; set; }

        public int DistributorId { get; set; }
        public string DistributorName { get; set; }
        public virtual Distributor Distributor { get; set; }

        public byte[] RowVersion { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }

        static Rate()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public Rate() { 
            this.Attachments = new HashSet<Attachment>();
        }

    }

    public partial class Cost : Rate {
        public string LotNumber { get; set; }
        public string SupplierPoNumber { get; set; }
        public virtual Lot Lot { get; set; }

        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        static Cost()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public Cost()
        {
            this.Attachments = new HashSet<Attachment>();
        }

        public Cost(Lot lot,Distributor distributor):this() {
            this.LotNumber = lot.LotNumber;
            this.SupplierPoNumber = lot.SupplierPoNumber;
            this.DistributorId = distributor.Id;
        }
    }

    public partial class Price : Rate {
        public int? PartInstanceId { get; set; }
        public virtual PartInstance PartInstance { get; set; }

        static Price()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public Price()
        {
            this.Attachments = new HashSet<Attachment>();
        }
    }
}
