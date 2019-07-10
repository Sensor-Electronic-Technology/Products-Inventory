namespace Inventory.Common.EntityLayer.Model.Entities {
    using EntityFramework.Triggers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Contact : IEntityWithTracking {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string Extension { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public int? ManufacturerID { get; set; }
        public string ManufacturerName { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public int? DistributorId { get; set; }
        public string DistributorName { get; set; }
        public virtual Distributor Distributor { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        static Contact()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public Contact()
        {

        }

    }
}
