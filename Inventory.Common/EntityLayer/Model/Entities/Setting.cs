
namespace Inventory.Common.EntityLayer.Model.Entities {
    using EntityFramework.Triggers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    //TODO: Add Alert tracking

    public partial class Settings {
        public int Id { get; set; }
        public string WindowTheme { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
        public byte[] RowVersion { get; set; }

        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        static Settings()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }
    }
}
