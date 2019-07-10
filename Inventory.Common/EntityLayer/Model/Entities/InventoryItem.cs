namespace Inventory.Common.EntityLayer.Model.Entities {
    using EntityFramework.Triggers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class InventoryItem: IEntityWithTracking {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int? OgranizationId { get; set; }
        public virtual Organization Organization { get; set; }

        public int? WarehouseId { get; set; }
        public virtual Warehouse Warehouse { get; set; }

        public byte[] RowVersion { get; set; }

        public virtual ICollection<Manufacturer> Manufacturers { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<Instance> Instances { get; set; }

        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        static InventoryItem() {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public InventoryItem(){

            this.Manufacturers = new HashSet<Manufacturer>();
            this.Attachments = new HashSet<Attachment>();
            this.Instances = new HashSet<Instance>();
        }
    }

    public partial class Product: InventoryItem {
        public string CustomPartName { get; set; }
        public string LegacyName { get; set; }
        public int Total { get; set; }
        public bool Obsolete { get; set; }

        public int? ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }

        public virtual ICollection<Lot> Lots { get; set; }

        public Product()
        {
            this.Lots = new HashSet<Lot>();
            this.Obsolete = false;
        }

        public Product(string name, string description) : this()
        {
            this.Name = name;
            this.Description = description;
        }
    }

    public partial class Part : InventoryItem {

        public int? UsageId { get; set; }
        public virtual Usage Usage { get; set; }

//        public virtual ICollection<PartInstance> PartInstances { get; set; }
        public Part()
        {
            //this.PartInstances = new HashSet<ItemInstance>();
        }

        public Part(string name, string description) : this()
        {
            this.Name = name;
            this.Description = description;
        }
    }
}
