namespace Inventory.Common.EntityLayer.Model.Entities {
    using EntityFramework.Triggers;
    using System;
    using System.Collections.Generic;

    public abstract class Location : IEntityWithTracking {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<PartInstance> ItemsAtLocation { get; set; }

        static Location() {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public Location() {
            this.ItemsAtLocation = new HashSet<PartInstance>();
            this.Transactions = new HashSet<Transaction>();
        }
    }

    public partial class Warehouse : Location {
        public virtual ICollection<InventoryItem> StoredItems { get; set; }

        public Warehouse() {
            this.Transactions = new HashSet<Transaction>();
            this.StoredItems = new HashSet<InventoryItem>();
        }
    }

    public partial class Consumer : Location {

        public Consumer() {
            this.ItemsAtLocation = new HashSet<PartInstance>();
            this.Transactions = new HashSet<Transaction>();
        }
    }
}
