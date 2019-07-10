namespace Inventory.Common.EntityLayer.Model.Entities {
    using EntityFramework.Triggers;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class Category : IEntityWithTracking {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] RowVersion { get; set; }

        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        static Category()
        {
            Triggers<IEntityWithTracking>.Inserting += e => e.Entity.Inserted = DateTime.UtcNow;
            Triggers<IEntityWithTracking>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
        }

        public Category() {

        }
    }

    public partial class Organization:Category {
        public virtual ICollection<InventoryItem> InventoryItems { get; set; }

        public Organization()
        {
            this.InventoryItems = new HashSet<InventoryItem>();
        }

        public Organization(string name,string description):this() {
            this.Name = name;
            this.Description = description;
        }
    }

    public partial class Condition : Category {
        public virtual ICollection<PartInstance> PartInstances { get; set; }

        public Condition()
        {
            this.PartInstances = new HashSet<PartInstance>();
        }
    }

    public partial class Usage : Category {
        public virtual ICollection<Part> Parts { get; set; }

        public Usage()
        {
            this.Parts = new HashSet<Part>();
        }
    }

    public partial class ProductType:Category {
        public virtual ICollection<Product> Products { get; set; }

        public ProductType()
        {
            this.Products = new HashSet<Product>();
        }
    }

    public partial class PartType : Category {
        public virtual ICollection<PartInstance> PartInstances { get; set; }

        public PartType()
        {
            this.PartInstances = new HashSet<PartInstance>();
        }
    }

    public partial class Rank : Category {
        //public virtual ICollection<ProductInstance> ProductInstances { get; set; }

        public Rank() {
            //this.ProductInstances = new HashSet<ProductInstance>();
        }
    }
}
