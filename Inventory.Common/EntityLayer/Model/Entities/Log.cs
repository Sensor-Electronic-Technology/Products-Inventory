namespace Inventory.Common.EntityLayer.Model.Entities {
    using EntityFramework.Triggers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Log : IEntityWithTracking {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserName { get; set; }
        public int SessionId { get; set; }
        public string EntityType { get; set; }
        public string EntityName { get; set; }
        public string Operation { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        static Log() {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public Log() {

        }

        public Log(string userName, string type, string entityName, string operation, DateTime now) {
            this.UserName = userName;
            this.EntityType = type;
            this.EntityName = entityName;
            this.Operation = operation;
            this.TimeStamp = now;
        }
    }
}
