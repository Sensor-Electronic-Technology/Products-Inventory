namespace Inventory.Common.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Inventory.Common.EntityLayer.Model.Entities;
    using Inventory.Common.EntityLayer.Model;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<InventoryContext> {

        public static string[] PartNames = { "CUD1AF4C ", "CUD1GF1A", "CUD5GF1A" };
        public static string basePo = "MC-";
        public static string[] wave = { "c2", "b2", "a3" };
        public static string[] power = { "A05", "A10", "A11" };
        public static string[] voltage = { "a", "b", "e" };

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;            
        }

        protected override void Seed(InventoryContext context)
        {

            //this.Generate(context);
            //this.GenerateDefaults(context);
            //this.GeneratePermissions(context);
        }

        private void GenerateDefaults(InventoryContext context) {

            var org = new Organization() { Name = "Products", Description = "Generic Products Organization Category" };
            var warehouse = new Warehouse() { Name = "Products", Description = "Generic Products Warehouse" };
            var manufacturer = new Manufacturer("SVC", "Parent Company Manufacturer", "None");

            context.Locations.Add(warehouse);
            context.Categories.Add(org);
            context.Manufacturers.Add(manufacturer);

            //Create Distributor

            Distributor distributor = new Distributor();
            distributor.Name = "SVC";
            distributor.Description = "Parent Company Distributor";

            context.Distributors.Add(distributor);
            context.SaveChanges();

            Consumer consumer = new Consumer();
            consumer.Name = "Customer";
            consumer.Description = "Generic Product Consumer";

            context.Locations.Add(consumer);
            context.SaveChanges();
        }

        private void GeneratePermissions(InventoryContext context) {
            Permission permission1 = new Permission() {
                Name = "InventoryAdminAccount",
                Description = "Full Inventory Privileges and User Control"
            };

            Permission permission2 = new Permission() {
                Name = "InventoryUserAccount",
                Description = "Inventory View Only"
            };

            Permission permission3 = new Permission() {
                Name = "InventoryUserFullAccount",
                Description = "Full Inventory Privileges"
            };

            Permission permission4 = new Permission() {
                Name = "InventoryUserLimitedAccount",
                Description = "Inventory Check In/Check Out/Create"
            };

            context.Permissions.Add(permission1);
            context.Permissions.Add(permission2);
            context.Permissions.Add(permission3);
            context.Permissions.Add(permission4);
            context.SaveChanges();
        }

        //private void Generate(InventoryContext context)
        //{
        //    Permission permission1 = new Permission() {
        //        Name = "InventoryAdminAccount",
        //        Description = "Full Inventory Privileges and User Control"
        //    };

        //    Permission permission2 = new Permission() {
        //        Name = "InventoryUserAccount",
        //        Description = "Inventory View Only"
        //    };

        //    Permission permission3 = new Permission() {
        //        Name = "InventoryUserFullAccount",
        //        Description = "Full Inventory Privileges"
        //    };

        //    Permission permission4 = new Permission() {
        //        Name = "InventoryUserLimitedAccount",
        //        Description = "Inventory Check In/Check Out/Create"
        //    };

        //    context.Permissions.Add(permission1);
        //    context.Permissions.Add(permission2);
        //    context.Permissions.Add(permission3);
        //    context.Permissions.Add(permission4);
        //    context.Permissions.Add(permission4);
        //    context.SaveChanges();

        //    Organization orgCat0 = new Organization() {
        //        Name = "Products",
        //        Description = "It's the facility stuff!",
        //    };

        //    Organization orgCat1 = new Organization() {
        //        Name = "Facility",
        //        Description = "It's the facility stuff!",
        //    };

        //    Organization orgCat2 = new Organization() {
        //        Name = "IT"
        //    };

        //    Organization orgCat3 = new Organization() {
        //        Name = "Epi"
        //    };

        //    Condition condition1 = new Condition() {
        //        Name = "New"
        //    };

        //    Condition condition2 = new Condition() {
        //        Name = "Used"
        //    };

        //    Condition condition3 = new Condition() {
        //        Name = "Refurbished"
        //    };

        //    Condition condition4 = new Condition() {
        //        Name = "Quarantined"
        //    };

        //    ProductType packageType1 = new ProductType() {
        //        Name = "CA3535",
        //        Description = "3535 SMD Package"
        //    };

        //    ProductType packageType2 = new ProductType() {
        //        Name = "TO39",
        //        Description = "TO39 Package"
        //    };

        //    ProductType packageType3 = new ProductType() {
        //        Name = "WICOP",
        //        Description = "Chip"
        //    };

        //    PartType partType1 = new PartType() {
        //        Name = "600g",
        //        Description = "600 gram bubbler"
        //    };

        //    PartType partType2 = new PartType() {
        //        Name = "300g",
        //        Description = "300 gram bubbler"
        //    };

        //    Usage usage1 = new Usage() {
        //        Name = "A Systems"
        //    };

        //    Usage usage2 = new Usage() {
        //        Name = "B Systems"
        //    };

        //    Usage usage3 = new Usage() {
        //        Name = "C Systems"
        //    };

        //    Usage usage4 = new Usage() {
        //        Name = "Digital Systems",
        //        Description = "Parts for digital type systems"
        //    };

        //    Usage usage5 = new Usage() {
        //        Name = "Analog Systems",
        //        Description = "Parts for analog type systems"
        //    };

        //    context.Categories.Add(orgCat0);
        //    context.Categories.Add(orgCat2);
        //    context.Categories.Add(orgCat1);
        //    context.Categories.Add(orgCat3);

        //    context.Categories.Add(condition1);
        //    context.Categories.Add(condition2);
        //    context.Categories.Add(condition3);
        //    context.Categories.Add(condition4);

        //    context.Categories.Add(packageType1);
        //    context.Categories.Add(packageType2);
        //    context.Categories.Add(packageType3);
        //    context.Categories.Add(partType1);
        //    context.Categories.Add(partType2);

        //    context.Categories.Add(usage1);
        //    context.Categories.Add(usage2);
        //    context.Categories.Add(usage3);
        //    context.Categories.Add(usage4);
        //    context.Categories.Add(usage5);


        //    context.SaveChanges();

        //    context.Locations.Add(new Warehouse() { Name = "Product Inventory Room A", Description = "Product Storage A" });
        //    context.Locations.Add(new Warehouse() { Name = "Product Inventory Room B", Description = "Product Storage B"});
        //    context.Locations.Add(new Warehouse() { Name = "Facility Storage", Description = "Facility Storage Racks"});
        //    context.Locations.Add(new Warehouse() { Name = "Epi Storage", Description = "Epi Storage Closet" });
        //    context.Locations.Add(new Warehouse() { Name = "Gas Bay", Description = "Gas Bay Room"});

        //    context.Locations.Add(new Consumer() { Name = "System B01", Description = "Epi System" });
        //    context.Locations.Add(new Consumer() { Name = "System B02", Description = "Epi System" });
        //    context.Locations.Add(new Consumer() { Name = "System A01", Description = "Epi System" });
        //    context.Locations.Add(new Consumer() { Name = "System A02", Description = "Epi System"});
        //    context.Locations.Add(new Consumer() { Name = "System C1", Description = "Epi System"});

        //    context.Locations.Add(new Consumer() { Name = "Customer", Description = "Generic Customer"});
        //    context.Locations.Add(new Consumer() { Name = "Supplier", Description = "Generic Supplier" });
        //    context.Locations.Add(new Warehouse() {Name = "SETi", Description = "Default Warehouse" });
        //    context.Locations.Add(new Consumer() { Name = "NOT OUT", Description = "Default Consumer" });
        //    //Warehouse warehouseDefault = new Warehouse() { Name = "SETi", Description = "Default Warehouse" };
        //    //Consumer consumerDefault = new Consumer() { Name = "NOT OUT", Description = "Default Consumer" };
        //    context.SaveChanges();
        //    //Generate Products
        //    for(int i = 0; i < 3; i++) {

        //        Product product = new Product() {
        //            Name = PartNames[i],
        //            Organization = orgCat0,
        //            ProductType = packageType1,
        //            Warehouse = context.Locations.OfType<Warehouse>().FirstOrDefault(location => location.Name == "Product Inventory Room A")
        //        };

        //        context.InventoryItems.Add(product);

        //        Lot lot = new Lot() {
        //            Recived = DateTime.UtcNow,
        //            SupplierPoNumber = basePo + DateTime.UtcNow.Year.ToString() + DateTime.UtcNow.Month.ToString() + DateTime.UtcNow.Date.ToString(),
        //            Product = product,
        //            ProductName = product.Name
        //        };
        //        lot.LotNumber = lot.SupplierPoNumber + product.Name;
        //        context.Lots.Add(lot);

        //        for(int x = 0; x < 3; x++) {
        //            ProductInstance instance = new ProductInstance() {
        //                Wavelength = wave[i],
        //                Power = power[i],
        //                Voltage = voltage[i],
        //                Quantity = (i + 1) * 52,
        //                MinQuantity = 0,
        //                SafeQuantity = 0,
        //                Lot = lot
        //            };
        //            instance.Name = instance.Wavelength + instance.Power + instance.Voltage;
        //            context.Instances.Add(instance);
        //        }
        //    }
        //    context.SaveChanges();
        //    Unit grams = new Unit("Grams", "g", 1, 1);
        //    Unit milligrams = new Unit("Milligrams", "mg", 10, -3);

        //    context.Units.Add(grams);
        //    context.Units.Add(milligrams);

        //    Parameter net = new Parameter() {
        //        Name = "Net Weight",
        //        Description = "",
        //        Unit = grams
        //    };

        //    Parameter gross = new Parameter() {
        //        Name = "Gross Weight",
        //        Description = "",
        //        Unit = grams
        //    };

        //    Parameter measured = new Parameter() {
        //        Name = "Measured Weight",
        //        Description = "",
        //        Unit = grams
        //    };

        //    Parameter remaining = new Parameter() {
        //        Name = "Remaining Weight",
        //        Description = "",
        //        Unit = grams
        //    };

        //    context.Parameters.Add(net);
        //    context.Parameters.Add(gross);
        //    context.Parameters.Add(measured);
        //    context.Parameters.Add(remaining);


        //    var warehouse = context.Locations.OfType<Warehouse>().FirstOrDefault(w => w.Name == "Facility Storage");
        //    if(warehouse != null) {

        //        Part bubbler = new Part("Bubbler", "");
        //        bubbler.Organization = orgCat1;
        //        context.InventoryItems.Add(bubbler);
        //        warehouse.StoredItems.Add(bubbler);

        //        for(int i = 0; i < 10; i++) {
        //            string name = i % 2 == 0 ? "TMA" : "TMG";
        //            Condition condition = i % 2 == 0 ? condition1 : condition2;
        //            PartInstance instance = new PartInstance(bubbler, name, "100000610" + i, "1607Al1" + i * 52, "");
        //            instance.Quantity = 1;
        //            instance.MinQuantity = 0;
        //            instance.MinQuantity = 0;
        //            instance.Condition = condition;
        //            instance.PartType = i % 2 == 0 ? partType2 : partType1;


        //            context.Instances.Add(instance);
        //            instance.CurrentLocation = warehouse;
        //            warehouse.ItemsAtLocation.Add(instance);
        //            context.Entry<Warehouse>(warehouse).State = EntityState.Modified;

        //            InstanceParameter bubbler_net = new InstanceParameter(instance, net) {
        //                Value = i % 2 == 0 ? 380 : 600,
        //                MinValue = 0,
        //                SafeValue = 0,
        //                Tracked = false,
        //                PartInstance = instance
        //            };

        //            InstanceParameter bubbler_gross = new InstanceParameter(instance, gross) {
        //                Value = i % 2 == 0 ? 2200 + (i * 3) : 2400 + (i * 3),
        //                MinValue = 0,
        //                SafeValue = 0,
        //                Tracked = false,
        //                PartInstance = instance
        //            };

        //            InstanceParameter bubbler_measured = new InstanceParameter(instance, measured) {
        //                Value = i % 2 == 0 ? 1800 + (i * 3) : 1900 + (i * 3),
        //                MinValue = i % 2 == 0 ? 1800 : 1900,
        //                SafeValue = i % 2 == 0 ? 1800 : 1900,
        //                Tracked = true,
        //                PartInstance = instance
        //            };

        //            InstanceParameter bubbler_remaining = new InstanceParameter(instance, remaining) {
        //                Value = i % 2 == 0 ? 300 + (i * 3) : 400 + (i * 3),
        //                MinValue = 150,
        //                SafeValue = 100,
        //                Tracked = false,
        //                PartInstance = instance
        //            };
        //            context.InstanceParameters.Add(bubbler_net);
        //            context.InstanceParameters.Add(bubbler_gross);
        //            context.InstanceParameters.Add(bubbler_measured);
        //            context.InstanceParameters.Add(bubbler_remaining);
        //        }

        //    }
        //}
    }
}
