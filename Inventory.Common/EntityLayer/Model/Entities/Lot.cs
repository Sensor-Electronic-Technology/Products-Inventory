namespace Inventory.Common.EntityLayer.Model.Entities {
    using System;
    using System.Collections.Generic;
    using EntityFramework.Triggers;

    public partial class Lot : IEntityWithTracking {
        public string LotNumber { get; set; }
        public string SupplierPoNumber { get; set; }
        public DateTime? Recieved { get; set; }
        public DateTime? ManufacturedDate { get; set; }
        public bool Quarantined { get; set; }
        public bool Obsolete { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public virtual Product Product  { get; set; }

        public int? CostId { get; set; }
        public virtual Cost Cost { get; set; }

        public byte[] RowVersion { get; set; }

        public virtual ICollection<ProductInstance> ProductInstances { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }

        //Tracking
        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        static Lot()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public Lot() {
            this.ProductInstances = new HashSet<ProductInstance>();
            this.Attachments = new HashSet<Attachment>();
            this.Quarantined = false;
            this.Obsolete = false;
        }
    }

}
