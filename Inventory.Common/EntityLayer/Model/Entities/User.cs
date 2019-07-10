namespace Inventory.Common.EntityLayer.Model.Entities {
    using EntityFramework.Triggers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class User: IEntityWithTracking {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Extension { get; set; }
        public InventorySoftwareType SoftwareVersion { get; set; }
        public bool StorePassword { get; set; }
        public string EncryptedPassword { get; set; }
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
        public byte[] RowVersion { get; set; }

        public int? PermissionId { get; set; }
        public virtual Permission Permission { get; set; }

        public int? SettingId { get; set; }
        public virtual Settings Settings { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
        public virtual ICollection<ProductReservation> ProductReservations { get; set; }
        
        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        static User()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public User()
        {
            this.Sessions = new HashSet<Session>();
            this.ProductReservations = new HashSet<ProductReservation>();
        }
    }
}
