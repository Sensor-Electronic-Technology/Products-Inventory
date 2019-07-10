namespace Inventory.Common.EntityLayer.Model.Entities {
    using EntityFramework.Triggers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    public partial class Manufacturer: IEntityWithTracking {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<InventoryItem> InventoryItems { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }



        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        static Manufacturer()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public Manufacturer()
        {
            this.InventoryItems = new HashSet<InventoryItem>();
            this.Contacts = new HashSet<Contact>();
            this.Attachments = new HashSet<Attachment>();
        }

        public Manufacturer(string name,string description,string comments) : this()
        {
            this.Name = name;
            this.Description = description;
            this.Comments = comments;
        }
    }
}
