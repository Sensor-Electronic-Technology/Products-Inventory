namespace Inventory.Common.EntityLayer.Model {
    using System;
    using System.Linq;
    using System.Data.Entity;
    using Inventory.Common.EntityLayer.Model.Entities;
    using System.ComponentModel.DataAnnotations.Schema;
    using EntityFramework.Triggers;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Threading.Tasks;

    public class InventoryContext : DbContextWithTriggers {
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Instance> Instances { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Lot> Lots { get; set; }
        public DbSet<Distributor> Distributors { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<InstanceParameter> InstanceParameters { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<ProductReservation> ProductReservations { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Log> Logs { get; set; }

        public InventoryContext()
            : base("name=InventoryContext") {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
            //Configuration.ProxyCreationEnabled = true;
            //Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<Lot>()
                .HasKey(k => new { k.LotNumber, k.SupplierPoNumber });

            modelBuilder.Entity<Lot>()
                .Property(t => t.LotNumber)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Lot>()
                .Property(t => t.SupplierPoNumber)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            #region Concurrency Configuration

            modelBuilder.Entity<Attachment>()
                .Property(a => a.RowVersion).IsRowVersion();

            modelBuilder.Entity<Category>()
                .Property(a => a.RowVersion).IsRowVersion();

            modelBuilder.Entity<Contact>()
                .Property(a => a.RowVersion).IsRowVersion();

            modelBuilder.Entity<Distributor>()
                .Property(a => a.RowVersion).IsRowVersion();

            modelBuilder.Entity<Location>()
                .Property(a => a.RowVersion).IsRowVersion();

            modelBuilder.Entity<Lot>()
                .Property(a => a.RowVersion).IsRowVersion();

            modelBuilder.Entity<Manufacturer>()
                .Property(a => a.RowVersion).IsRowVersion();

            modelBuilder.Entity<InventoryItem>()
                .Property(a => a.RowVersion).IsRowVersion();

            modelBuilder.Entity<Instance>()
                .Property(a => a.RowVersion).IsRowVersion();

            modelBuilder.Entity<Permission>()
                .Property(a => a.RowVersion).IsRowVersion();

            modelBuilder.Entity<Rate>()
                .Property(a => a.RowVersion).IsRowVersion();

            modelBuilder.Entity<ProductReservation>()
                .Property(a => a.RowVersion).IsRowVersion();

            modelBuilder.Entity<Session>()
                .Property(a => a.RowVersion).IsRowVersion();

            modelBuilder.Entity<Settings>()
                .Property(a => a.RowVersion).IsRowVersion();

            modelBuilder.Entity<Transaction>()
                .Property(a => a.RowVersion).IsRowVersion();

            modelBuilder.Entity<User>()
                .Property(a => a.RowVersion).IsRowVersion();

            #endregion

            modelBuilder.Entity<Lot>()
                .HasRequired(e => e.Product)
                .WithMany(e => e.Lots)
                .HasForeignKey(e => e.ProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lot>()
                .HasMany(e => e.ProductInstances)
                .WithRequired(e => e.Lot)
                .HasForeignKey(e => new { e.LotNumber, e.SupplierPoNumber})
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductReservation>()
                .HasRequired(e => e.ProductInstance)
                .WithMany(e => e.ProductReservations)
                .HasForeignKey(e => e.ProductInstancdId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductReservation>()
                .HasRequired(e => e.User)
                .WithMany(e => e.ProductReservations)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<InventoryItem>()
                .HasOptional(e => e.Organization)
                .WithMany(e => e.InventoryItems)
                .HasForeignKey(e => e.OgranizationId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Part>()
                .HasOptional(e => e.Usage)
                .WithMany(e => e.Parts)
                .HasForeignKey(e => e.UsageId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasOptional(e => e.ProductType)
                .WithMany(e => e.Products)
                .HasForeignKey(e => e.ProductTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Parameter>()
                .HasRequired(e => e.Unit)
                .WithMany(e => e.Parameters)
                .HasForeignKey(e => e.UnitId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<InstanceParameter>()
                .HasRequired(e => e.Parameter)
                .WithMany(e => e.InstanceParameters)
                .HasForeignKey(e => e.ParameterId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<InstanceParameter>()
                .HasRequired(e => e.PartInstance)
                .WithMany(e => e.InstanceParameters)
                .HasForeignKey(e => e.PartInstanceId)
                .WillCascadeOnDelete(false);
            
            modelBuilder.Entity<Lot>()
                .HasOptional(e => e.Cost)
                .WithRequired(e => e.Lot)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Cost>()
                .HasRequired(e => e.Lot)
                .WithOptional(e => e.Cost)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PartInstance>()
                .HasOptional(e => e.CurrentLocation)
                .WithMany(e => e.ItemsAtLocation)
                .HasForeignKey(e => e.CurrentLocationId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PartInstance>()
                .HasOptional(e => e.PartType)
                .WithMany(e => e.PartInstances)
                .HasForeignKey(e => e.PartTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PartInstance>()
                .HasOptional(e => e.Condition)
                .WithMany(e => e.PartInstances)
                .HasForeignKey(e => e.ConditionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PartInstance>()
                .HasMany(e => e.Prices)
                .WithOptional(e => e.PartInstance)
                .HasForeignKey(e => e.PartInstanceId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Instance>()
                .HasRequired(e => e.InventoryItem)
                .WithMany(e => e.Instances)
                .HasForeignKey(e => e.InventoryItemId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Location>()
                .HasMany(e => e.Transactions)
                .WithOptional(e => e.Location)
                .HasForeignKey(e => e.LocationId)
                .WillCascadeOnDelete(false);
            
            modelBuilder.Entity<InventoryItem>()
                .HasOptional(e => e.Warehouse)
                .WithMany(e => e.StoredItems)
                .HasForeignKey(e => e.WarehouseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Warehouse>()
                .HasMany(e => e.StoredItems)
                .WithOptional(e => e.Warehouse)
                .HasForeignKey(e => e.WarehouseId)
                .WillCascadeOnDelete(false);
             
            modelBuilder.Entity<Rate>()
                .HasRequired(e => e.Distributor)
                .WithMany(e => e.Rates)
                .HasForeignKey(e => e.DistributorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Manufacturer>()
                .HasMany(e => e.InventoryItems)
                .WithMany(e => e.Manufacturers);

            modelBuilder.Entity<Transaction>()
                .HasRequired(e => e.Session)
                .WithMany(e => e.Transactions)
                .HasForeignKey(e => e.SessionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Transaction>()
                .HasRequired(e => e.Instance)
                .WithMany(e => e.Transactions)
                .HasForeignKey(e => e.InstanceId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Transaction>()
                .HasOptional(e => e.OutgoingTransaction)
                .WithOptionalPrincipal();

            modelBuilder.Entity<Transaction>()
                .HasOptional(e => e.Location)
                .WithMany(e => e.Transactions)
                .HasForeignKey(e => e.LocationId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PartTransaction>()
                .HasOptional(e => e.InstanceParameter)
                .WithMany(e => e.PartTransactions)
                .HasForeignKey(e => e.InstanceParameterId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<User>()
                .HasOptional(e => e.Settings)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<User>()
                .HasOptional(e => e.Permission)
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.PermissionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Sessions)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Attachment>()
                .HasOptional(e => e.InventoryItem)
                .WithMany(e => e.Attachments)
                .HasForeignKey(e => e.InventoryItemId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Distributor>()
                .HasMany(e => e.Attachments)
                .WithOptional(e=>e.Distributor)
                .HasForeignKey(e => e.DistributorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Distributor>()
                .HasMany(e => e.Contacts)
                .WithOptional(e => e.Distributor)
                .HasForeignKey(e => e.DistributorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Manufacturer>()
                .HasMany(e => e.Contacts)
                .WithOptional(e => e.Manufacturer)
                .HasForeignKey(e => e.ManufacturerID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Attachment>()
                .Ignore(e => e.SourceReference);

            modelBuilder.Entity<Attachment>()
                .HasOptional(e => e.Manufacturer)
                .WithMany(e => e.Attachments)
                .HasForeignKey(e => e.ManufacturerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Attachment>()
                .HasOptional(e => e.Lot)
                .WithMany(e => e.Attachments)
                .HasForeignKey(e => new {e.LotNumber,e.SupplierPoNumber})
                .WillCascadeOnDelete(false);

        }//End model builder
    }
}



/***********************************************************************************************
*                                   Backup
***********************************************************************************************/

/***********************************************************************************************
*                                   DbSETS
***********************************************************************************************/

//public DbSet<Product> Products { get; set; }
//public DbSet<ProductInstance> ProductInstances { get; set; }
//public DbSet<Product_Category_Field> Product_Categories { get; set; }
//public DbSet<Part_Category_Field> Part_Categories { get; set; }

/***********************************************************************************************
*                                   Model Builder
***********************************************************************************************/

//modelBuilder.Entity<InventoryItem>()
//    .Map<Product>(m => m.Requires("Discriminator").HasValue("Product"))
//    .Map<Part>(m => m.Requires("Discriminator").HasValue("Part"));

//modelBuilder.Entity<Instance>()
//    .Map<ProductInstance>(m => m.Requires("Discriminator").HasValue("ProductInstance"))
//    .Map<PartInstance>(m => m.Requires("Discriminator").HasValue("PartInstance"));

//modelBuilder.Entity<Entity_Category_Field>()
//    .Map<Product_Category_Field>(m => m.Requires("Discriminator").HasValue("Product_Category_Field"))
//    .Map<Part_Category_Field>(m => m.Requires("Discriminator").HasValue("Part_Category_Field"));

//modelBuilder.Entity<Filter>()
//    .HasKey(k => k.FilterName);

//modelBuilder.Entity<Filter>()
//    .Property(t => t.FilterName)
//    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);