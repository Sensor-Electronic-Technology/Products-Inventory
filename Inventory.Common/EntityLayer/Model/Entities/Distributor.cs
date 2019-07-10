namespace Inventory.Common.EntityLayer.Model.Entities {
    using EntityFramework.Triggers;
    using System;
    using System.Collections.Generic;

    public partial class Distributor: IEntityWithTracking {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Rate> Rates { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }

        //Tracking
        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        static Distributor()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public Distributor()
        {
            this.Rates = new HashSet<Rate>();
            this.Contacts = new HashSet<Contact>();
            this.Attachments = new HashSet<Attachment>();
        }
    }
}
