namespace Inventory.Common.EntityLayer.Model.Entities {
    using System;
    using System.Collections.Generic;
    using EntityFramework.Triggers;

    public partial class Permission:IEntityWithTracking {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<User> Users { get; set; }

        //Tracking
        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        static Permission()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public Permission()
        {
            this.Users = new HashSet<User>();
        }
    }
}
