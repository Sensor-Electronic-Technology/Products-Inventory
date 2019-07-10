namespace Inventory.Common.EntityLayer.Model.Entities {
    using EntityFramework.Triggers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Attachment: IEntityWithTracking {
        public int Id { get; set; }
        public DateTime Created  { get; set; }
        public DateTime? ValidThough { get; set; }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string SourceReference { get; set; }
        public string FileReference { get; set; }
        public string Extension { get; set; }
        public bool Expires { get; set; }

        public string LotNumber { get; set; }
        public string SupplierPoNumber { get; set; }
        public virtual Lot Lot { get; set; }

        public int? InventoryItemId { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }

        public int? DistributorId { get; set; }
        public virtual Distributor Distributor { get; set; }
        
        public int? ManufacturerId { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
       
        public int? CostId { get; set; }
        public virtual Cost Rate { get; set; }

        public byte[] RowVersion { get; set; }

        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        static Attachment()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public Attachment() {
            
        }

        public Attachment(DateTime now,string name,string source)
        {
            this.Created = now;
            this.Name = name;
            this.SourceReference = source;
        }
    }
}
