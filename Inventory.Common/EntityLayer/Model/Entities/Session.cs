namespace Inventory.Common.EntityLayer.Model.Entities {
    using EntityFramework.Triggers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Session: IEntityWithTracking {
        public int Id { get; set; }
        public DateTime In { get; set; }
        public DateTime? Out { get; set; }

        public int? UserId { get; set; }
        public string UserName { get; set; }
        public virtual User User { get; set; }

        public byte[] RowVersion { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        static Session()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public Session()
        {
            this.Transactions = new HashSet<Transaction>();
        }

        public Session(User user):this()
        {
            this.In = DateTime.UtcNow;
            this.User = user;
            this.UserName = user.UserName;
        }
    }
}
