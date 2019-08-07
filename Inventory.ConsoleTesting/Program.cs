using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.Common.DataLayer;
using Inventory.Common.DataLayer.Providers;
using Console_Table;
using System.Reflection;
using Inventory.Common.DataLayer.Services;
using System.Dynamic;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Security.Cryptography;
using System.IO;
using Inventory.Common.ApplicationLayer;
using Inventory.Common.BuisnessLayer;
using System.Linq.Expressions;
using Inventory.Common.DataLayer.EntityDataManagers;
using Inventory.Common.DataLayer.EntityOperations;
using System.Data.Entity;

namespace Inventory.ConsoleTesting {
    public class Program {
        private static void Main(string[] args) {
            DeleteTransaction(254);
            //DeleteLotFix("190717DS-01-057-7890040", "0336162");
            //RankChangs();
            //FixTransaction(395);
            //GenerateTables generate = new GenerateTables();
            //generate.ReadFromFile();
            //generate.GeneratePackageTypes();
            //generate.CreatePartLots();

            //GenerateTransactions generate = new GenerateTransactions();
            //generate.ReadFromFile();
            //generate.Generate();

            //using (InventoryContext context = new InventoryContext()) {
            //    IUserService userService = new UserService();
            //    DomainManager domainManager = new DomainManager();
            //    UserServiceProvider userServiceProvider = new UserServiceProvider(context, domainManager);
            //    LogInService logInService = new LogInService(domainManager, userServiceProvider);
            //    var responce = logInService.LogInWithPassword("AElmendo", "Drizzle123!", false, InventorySoftwareType.PRODUCTS_SALES);
            //    userService = responce.Service;

            //    var consumer = context.Locations.OfType<Consumer>().FirstOrDefault(x => x.Name == "Customer");
            //    var warehouse = context.Locations.OfType<Warehouse>().FirstOrDefault(x => x.Name == "Products");

            //    AddMissingTransactions(context, userService.CurrentSession, InventoryAction.INCOMING, warehouse, "TUD69H1B", "260nm~266nm","190412DS-01-001","36139", "5/20/2019","","", 100);
                //AddMissingTransactions(context, userService.CurrentSession, InventoryAction.OUTGOING, consumer, "CUN06B1B", "p2B20Z6", "6/4/2019", "052019JB", "", 10);
                //AddMissingTransactions(context, userService.CurrentSession, InventoryAction.INCOMING, warehouse, "CUD4GF1B", "p1H2Z3", "6/4/2019", "", "", 200);
                //AddMissingTransactions(userService.CurrentSession, InventoryAction.OUTGOING, consumer,"p1H2Z3", "6/4/2019", "052019JB", "", 10);
                //Console.WriteLine("Should be added");
            //}




            //using(InventoryContext context = new InventoryContext()) {
            //    int ID = 120;
            //    var product = context.InventoryItems.OfType<Product>()
            //        .Include(e => e.Instances)
            //        .Include(e => e.Lots.Select(x=>x.ProductInstances))
            //        .FirstOrDefault(e => e.Id == ID);

            //    var rank = context.Instances.OfType<ProductInstance>().Include(e=>e.Lot).Include(e=>e.Transactions.Select(x=>x.Instance)).FirstOrDefault(e=>e.InventoryItemId==ID);

            //    var lot = context.Lots.Find(rank.LotNumber, rank.SupplierPoNumber);
            //    lot.ProductInstances.Remove(rank);

            //    rank.InventoryItem = null;
            //    rank.Lot = null;
            //    rank.CurrentLocation = null;


            //    rank.Transactions.ToList().ForEach(t => {
            //        t.Instance = null;
            //        context.Transactions.Remove(t);
            //    });
            //    rank.Transactions.Clear();
            //    rank.ProductReservations.Clear();

            //    context.InventoryItems.Remove(product);
            //    context.Instances.Remove(rank);

            //    context.SaveChanges();
            //    Console.WriteLine("ID: {0} Should be removed", ID);
            //}

            //using(InventoryContext context = new InventoryContext()) {
            //    var product = context.InventoryItems.OfType<Product>()
            //        .Include(e => e.Lots)
            //        .Include(e => e.Instances.Select(x => x.Transactions))
            //        .FirstOrDefault(e => e.Name == "TCD89B1C");
            //    if(product != null) {
            //        var rank = product.Instances.OfType<ProductInstance>().FirstOrDefault(x => x.Name == "280nm~290nm");
            //        if(rank != null) {
            //            var transaction = rank.Transactions.OfType<ProductTransaction>().FirstOrDefault(x => x.BuyerPoNumber == "Device failed testing");
            //            if(transaction != null) {
            //                rank.Quantity = 1;
            //                transaction.Quantity = 1;
            //                context.Entry<Transaction>(transaction).State = EntityState.Modified;
            //                context.Entry<Instance>(rank).State = EntityState.Modified;
            //                context.SaveChanges();
            //            } else {
            //                Console.WriteLine("Transaction Not Found");
            //            }
            //        } else {
            //            Console.WriteLine("Rank Not Found");
            //        }
            //    }
            //}


            //using(StreamWriter writer = new StreamWriter(@"C:\Lots.txt")) {
            //    foreach(var item in generate.Lots) {
            //        writer.WriteLine(item);
            //    }
            //}

            //using(StreamWriter writer = new StreamWriter(@"C:\Products.txt")) {
            //    foreach(var item in generate.uniqueNames) {
            //        writer.WriteLine(item);
            //    }
            //}

            //using(StreamWriter writer = new StreamWriter(@"C:\ProductsLots.txt")) {
            //    foreach(var item in generate.uniqueNameLots) {
            //        StringBuilder builder = new StringBuilder();
            //        builder.AppendFormat("({0},{1},{2})", item[0], item[1], item[2]);
            //        writer.WriteLine(builder.ToString());
            //    }
            //}
            //using(var context = new InventoryContext()) {

            //    var products = context.InventoryItems.OfType<Product>()
            //   .Include(e => e.Attachments)
            //   .Include(e => e.Lots)
            //   .Include(e => e.Warehouse)
            //   .Include(e => e.ProductType)
            //   .Include(e => e.Organization)
            //   .Include(e => e.Instances).ToList();

            //    foreach(var product in products) {
            //        Console.WriteLine("Product: {0}", product.Name);
            //        foreach(var rank in product.Instances.OfType<ProductInstance>()) {
            //            Console.WriteLine("Rank: {0} Unit Cost: {1}", rank.Name, rank.Lot.Cost.Amount);
            //        }
            //    }
            //    Console.WriteLine();
            //}

            //using(var context=new InventoryContext()) {
            //    var transaction = context.Transactions.OfType<ProductTransaction>().Include(e=>e.Instance).FirstOrDefault(x=>x.Id==148);
            //    transaction.Instance.Transactions.Clear();
            //    context.Transactions.Remove(transaction);

            //    context.SaveChanges();
            //}
            //ProductReservationDeleteTesting();
            //ProductReservationAddTesting();
            // Console.WriteLine("Should Be Done");

            //Console.ReadKey();
        }

        private static void RankChangs() {
            using (InventoryContext context = new InventoryContext()) {
                var lotEntity = context.Lots.Include(e => e.ProductInstances.Select(i => i.Transactions))
                    .Include(e => e.Cost).FirstOrDefault(x => x.LotNumber == "18A18P00DA-7890048" && x.SupplierPoNumber == "036185");
                var rank = lotEntity.ProductInstances.FirstOrDefault(x => x.Name == "jB25Z6");
                if (rank != null) {
                    //rank.Quantity = 35;

                    var t2 = rank.Transactions.OfType<ProductTransaction>().FirstOrDefault(x => x.InventoryAction == InventoryAction.OUTGOING);
                    //t2.Quantity = 65;
                    t2.TotalCost = t2.Quantity * t2.UnitCost;
                    context.Entry<Transaction>(t2).State = EntityState.Modified;
                    context.Entry<ProductInstance>(rank).State = EntityState.Modified;
                    context.SaveChanges();
                    Console.WriteLine("Should be done");
                    Console.ReadKey();
                } else {
                    Console.WriteLine("Null....");
                    Console.ReadKey();
                }
            }
        }

        private static void FixTransaction(int id) {
            using(InventoryContext context=new InventoryContext()) {
                var tran = context.Transactions.OfType<ProductTransaction>().Include(e => e.Instance).FirstOrDefault(e => e.Id == id);
                if (tran != null) {
                    tran.UnitCost = 2.24;
                    tran.TotalCost = tran.Quantity * tran.UnitCost;
                    context.Entry<Transaction>(tran).State = EntityState.Modified;
                    context.SaveChanges();
                    Console.WriteLine("Should be done");
                } else {
                    Console.WriteLine("Failed");
                }
                Console.ReadKey();
            }
        }

        private static void DeleteTransaction(int id) {
            using (InventoryContext context = new InventoryContext()) {
                var tran = context.Transactions.OfType<ProductTransaction>().Include(e => e.Instance).FirstOrDefault(e => e.Id == id);
                if (tran != null) {
                    var rank = context.Instances.OfType<ProductInstance>().Include(e => e.InventoryItem).FirstOrDefault(e => e.Id == tran.InstanceId);

                    if (rank != null) {
                        var product = context.InventoryItems.OfType<Product>().Include(e => e.Instances).FirstOrDefault(x => x.Id == rank.InventoryItemId);
                        if (product != null) {
                            //assumes was outgoing -= if incoming
                            rank.Quantity += tran.Quantity;
                            rank.Transactions.Remove(tran);
                            context.Entry<Instance>(rank).State = EntityState.Modified;
                            context.Transactions.Remove(tran);
                            context.SaveChanges();
                            Console.WriteLine("Should be done,Updating Quantities");
                            context.InventoryItems.OfType<Product>().Include(e => e.Instances).Include(e => e.Lots).ToList().ForEach(p=> {
                                product.Total = product.Instances.Sum(q => q.Quantity);
                            });
                            Console.WriteLine("Quantities up to date");
                            context.SaveChanges();
                        } else {
                            Console.WriteLine("Failed,prodcut was null");
                        }

                    } else {
                        Console.WriteLine("Failed,rank was null");
                    }

                } else {
                    Console.WriteLine("Failed,transaction was null");
                }
                Console.ReadKey();
            }
        }

        private static void DeleteLotFix(string lotNum,string po) {
            using (InventoryContext _context = new InventoryContext()) {
                _context.Lots.Load();
                _context.Instances.Load();
                _context.Rates.Load();
                _context.Lots.Include(e => e.ProductInstances.Select(x => x.Transactions)).Include(e => e.Cost).Load();
                var lotEntity = _context.Lots.Include(e => e.ProductInstances.Select(i => i.Transactions))
                        .Include(e => e.Cost).FirstOrDefault(x => x.LotNumber == lotNum && x.SupplierPoNumber == po);
                if (lotEntity != null) {
                    lotEntity.ProductInstances.ToList().ForEach(rank => {
                        rank.Transactions.ToList().ForEach(t => {
                            _context.Transactions.Remove(t);
                        });
                        _context.Instances.Remove(rank);
                    });
                    lotEntity.ProductInstances.Clear();
                    _context.Rates.Remove(lotEntity.Cost);
                    lotEntity.Cost = null;
                    lotEntity.Product = null;
                    _context.Lots.Remove(lotEntity);
                    _context.SaveChanges();
                    Console.WriteLine("Should be done!");
                    Console.ReadKey();
                } else {
                    Console.WriteLine("Lot is Null");
                    Console.ReadKey();
                }

            }
        }

        private static void FixLocationRepeats() {
            using(InventoryContext context = new InventoryContext()) {
                int id = 14;
                var loc = context.Locations.OfType<Warehouse>().Include(e => e.Transactions).FirstOrDefault(e => e.Id == 1);
                context.Transactions.OfType<ProductTransaction>().Include(e => e.Location).Where(e => e.LocationId == id).ToList().ForEach(transaction => {
                    Console.WriteLine("Transaction: {0} Changed", transaction.Id);
                    transaction.Location = loc;
                    transaction.LocationId = id;
                    context.Entry<Warehouse>(loc).State = EntityState.Modified;
                    context.SaveChanges();
                });

                context.Instances.OfType<ProductInstance>().Include(e => e.CurrentLocation).Where(e => e.CurrentLocation.Id == id).ToList().ForEach(rank => {
                    Console.WriteLine("Rank: {0} Changed", rank.Name);
                    rank.CurrentLocation = loc;
                    rank.CurrentLocationId = id;
                    context.Entry<ProductInstance>(rank).State = EntityState.Modified;
                    context.SaveChanges();
                });

                context.InventoryItems.OfType<Product>().Include(e => e.Warehouse).Where(e => e.WarehouseId == id).ToList().ForEach(product => {
                    Console.WriteLine("Product: {0} Changed", product.Name);
                    product.Warehouse = loc;
                    product.WarehouseId = id;
                    context.Entry<Product>(product).State = EntityState.Modified;
                    context.SaveChanges();
                });

                var location = context.Locations.OfType<Warehouse>().Include(e => e.Transactions).FirstOrDefault(e => e.Id == id);
                location.Transactions.Clear();
                context.Locations.Remove(location);
                context.SaveChanges();
            }
        }

        private static void TestingDataSummary() {
            using(var context = new InventoryContext()) {
                var products=context.InventoryItems.OfType<Product>()
                        .AsNoTracking()
                        .Include(e => e.Attachments)
                        .Include(e => e.Lots.Select(x => x.ProductInstances))
                        .Include(e => e.Lots.Select(x => x.Cost))
                        .Include(e => e.Warehouse)
                        .Include(e => e.ProductType)
                        .Include(e => e.Organization)
                        .Include(e => e.Manufacturers);
                double total = 0;
                foreach(var product in products) {
                    var pTotal=product.Lots.Sum(lot => {
                        var quantity=lot.ProductInstances.Sum(rank => rank.Quantity);
                        return quantity * lot.Cost.Amount;
                    });
                    total += pTotal;
                    Console.WriteLine("Product: {0} Cost: {1}",product.Name,pTotal);
                }
                Console.WriteLine("Grand Total: {0}",total);
            }
        }

        private static void AddMissingTransactions(InventoryContext context,Session session,InventoryAction action,Location location,string partName,string rankName,string date,string po,string rma,int quantity) {
                
                var tran = context.Transactions.Create<ProductTransaction>();
                //tran.Lot = lot;
                var rank = context.Instances.OfType<ProductInstance>()
                    .Include(e=>e.InventoryItem)
                    .FirstOrDefault(x => x.Name == rankName && x.InventoryItem.Name==partName);
                if(rank != null) {
                    tran.TimeStamp = DateTime.Parse(date);
                    tran.InstanceId = rank.Id;
                    tran.ProductName = rank.InventoryItem.Name;
                    tran.Quantity = quantity;
                    tran.BuyerPoNumber = po;
                    tran.RMA_Number = rma;
                    tran.Location = location;
                    tran.InventoryAction = action;
                    tran.IsReturning = false;
                    tran.Session = session;
                    context.Transactions.Add(tran);
                    context.SaveChanges();
                    Console.WriteLine("Created! Part: {0})",tran.ProductName);
                } else {
                    Console.WriteLine("Rank: {0} not found in Lot.  Press Any Key To Continue");
                    Console.ReadKey();
                }
        }

        private static void AddMissingTransactions(InventoryContext context, Session session, InventoryAction action, Location location, string partName, string rankName,string lotNumber,string Ponum,string date, string po, string rma, int quantity) {
            var tran = context.Transactions.Create<ProductTransaction>();
            //tran.Lot = lot;
            //var rank = context.Instances.OfType<ProductInstance>()
            //    .Include(e => e.InventoryItem)
            //    .FirstOrDefault(x => x.Name == rankName && x.InventoryItem.Name == partName);
            var lot = context.Lots.Include(e=>e.ProductInstances.Select(r=>r.InventoryItem)).FirstOrDefault(e=>e.LotNumber==lotNumber && e.SupplierPoNumber==Ponum);
            if (lot != null) {
                var rank = lot.ProductInstances.FirstOrDefault(e => e.Name == rankName);
                if (rank != null) {
                    tran.TimeStamp = DateTime.Parse(date);
                    tran.InstanceId = rank.Id;
                    tran.ProductName = rank.InventoryItem.Name;
                    tran.Quantity = quantity;
                    tran.BuyerPoNumber = po;
                    tran.RMA_Number = rma;
                    tran.Location = location;
                    tran.InventoryAction = action;
                    tran.IsReturning = false;
                    tran.Session = session;
                    context.Transactions.Add(tran);
                    context.SaveChanges();
                    Console.WriteLine("Created! Part: {0})", tran.ProductName);
                    Console.ReadKey();
                } else {
                    Console.WriteLine("Rank: {0} not found in Lot.  Press Any Key To Continue");
                    Console.ReadKey();
                }

            } else {
                Console.WriteLine("Rank: {0} not found in Lot.  Press Any Key To Continue");
                Console.ReadKey();
            }
        }

        private static void ProductReservationAddTesting() {
            using(var context=new InventoryContext()) {
                IUserService userService = new UserService();
                DomainManager domainManager = new DomainManager();
                UserServiceProvider userServiceProvider = new UserServiceProvider(context, domainManager);
                LogInService logInService = new LogInService(domainManager, userServiceProvider);
                var responce = logInService.LogInWithPassword("AElmendo", "Drizzle123!", false, InventorySoftwareType.PRODUCTS_SALES);
                userService = responce.Service;
                ReservationOperations operations = new ReservationOperations(context, userService);
                var rank = context.Instances.AsNoTracking().OfType<ProductInstance>()
                    .Include(e => e.ProductReservations)
                    .Include(e => e.Lot)
                    .FirstOrDefault(e => e.Id == 422);
                if(rank != null) {
                    ProductReservation reservation = new ProductReservation(rank,50);
                    reservation.Expiration = reservation.TimeStamp.AddMonths(1);
                    reservation.RMANumber = "AE-8954-243";
                    reservation.BuyerPoNumber = "AE-BR-234";
                    reservation.Customer = "Andrew E Inc";
                    var added = operations.Add(reservation);
                    if(added != null) {
                        Console.WriteLine("Should Be added");

                    } else {
                        Console.WriteLine("Failed!");
                    }
                } else {
                    Console.WriteLine("Couldn't Find Rank");
                }
            }
        }

        private static void ProductReservationUpdateTesting() {
            using(var context = new InventoryContext()) {
                IUserService userService = new UserService();
                DomainManager domainManager = new DomainManager();
                UserServiceProvider userServiceProvider = new UserServiceProvider(context, domainManager);
                LogInService logInService = new LogInService(domainManager, userServiceProvider);
                var responce = logInService.LogInWithPassword("AElmendo", "Drizzle123!", false, InventorySoftwareType.PRODUCTS_SALES);
                userService = responce.Service;

                ReservationOperations operations = new ReservationOperations(context, userService);
                ReservationProvider provider = new ReservationProvider(context, userService);
                var reservation = provider.GetEntity(e => e.ProductInstancdId == 422);

                if(reservation!=null) {
                    reservation.Expiration = reservation.TimeStamp.AddMonths(2);
                    reservation.Quantity = 500;
                    var updated=operations.Update(reservation);
                    if(updated != null) {
                        Console.WriteLine("Success!");
                    } else {
                        Console.WriteLine("Update Failed");
                    }
                } else {
                    Console.WriteLine("Couldn't Find Reservation");
                }
            }
        }

        private static void ProductReservationDeleteTesting() {
            using(var context = new InventoryContext()) {
                IUserService userService = new UserService();
                DomainManager domainManager = new DomainManager();
                UserServiceProvider userServiceProvider = new UserServiceProvider(context, domainManager);
                LogInService logInService = new LogInService(domainManager, userServiceProvider);
                var responce = logInService.LogInWithPassword("AElmendo", "Drizzle123!", false, InventorySoftwareType.PRODUCTS_SALES);
                userService = responce.Service;

                ReservationOperations operations = new ReservationOperations(context, userService);
                ReservationProvider provider = new ReservationProvider(context, userService);
                var reservation = provider.GetEntity(e => e.ProductInstancdId == 422);

                if(reservation != null) {
                    var deleted=operations.Delete(reservation);
                    if(deleted != null) {
                        Console.WriteLine("Success!");
                    } else {
                        Console.WriteLine("Delete Failed");
                    }
                } else {
                    Console.WriteLine("Couldn't Find Rank or Reservation");
                }
            }
        }

        private static void TestingNewLotCreation() {
            using(InventoryContext context = new InventoryContext()) {
                var product = context.InventoryItems.OfType<Product>().FirstOrDefault(x => x.Name == "CUD9GF1A");
                var distributor = context.Distributors.FirstOrDefault(x => x.Name == "SVC");

                Lot lot = new Lot();
                lot.LotNumber = "4444444444444444";
                lot.SupplierPoNumber = "999999999999";
                lot.Recieved = DateTime.Now;
                lot.Product = product;
                Cost cost = new Cost();
                cost.Amount = 2.75;
                cost.TimeStamp = DateTime.Now;
                //cost.Lot = lot;
                lot.Cost = cost;
                context.Lots.Add(lot);
                List<ProductInstance> ranks = new List<ProductInstance>() {
                    new ProductInstance(){Name="ca1235fhg"},
                    new ProductInstance(){Name="ca12465fhg"},
                    new ProductInstance(){Name="ca14123fhg"},
                    new ProductInstance(){Name="ca1211fhg"},
                    new ProductInstance(){Name="ca9999fhg"},
                };
                ranks.ForEach(rank => {
                    rank.Lot = lot;
                    rank.InventoryItem = product;
                    context.Instances.Add(rank);
                });

                context.SaveChanges();
                Console.WriteLine("Should be done");
                Console.ReadKey();
            }
        }

        private static void QuantityByRankByPart() {
            using(var context = new InventoryContext()) {

                var products = context.InventoryItems.OfType<Product>()
               .Include(e => e.Attachments)
               .Include(e => e.Lots)
               .Include(e => e.Warehouse)
               .Include(e => e.ProductType)
               .Include(e => e.Organization)
               .Include(e => e.Instances.OfType<ProductInstance>().Select(x => x.Lot.Cost)).ToList();

                context.InventoryItems.OfType<Product>()
                    .Include(e => e.Instances)
                    .Load();
                var product = context.InventoryItems.OfType<Product>().Include(e => e.Instances).FirstOrDefault(x => x.Name == "CUD7GF1A");
                var unique = (from item in product.Instances
                              select item.Name).Distinct();

                Dictionary<string, int> countByRank = new Dictionary<string, int>();

                foreach(var item in unique) {
                    int count = product.Instances.Where(x => x.Name == item).Sum(e => e.Quantity);
                    countByRank.Add(item, count);
                }

                Console.WriteLine("Unique: {0}  Base: {1}", unique.Count(), product.Instances.Count);
                foreach(var item in countByRank) {
                    Console.WriteLine("Rank: {0}  Quantity: {1}", item.Key, item.Value);
                }


                //var grouped = from product in context.InventoryItems.Where(x => x.Name == "CUD7GF1A")
                //              from rank in product.Instances;
            }
        }

        private static void CreateOrgAndWarehouse() {
            var org=new Organization() { Name = "Products", Description = "Generic Products Organization Category" };
            var warehouse = new Warehouse() { Name = "Products", Description = "Generic Products Warehouse" };
            using(var context=new InventoryContext()) {
                context.Locations.Add(warehouse);
                context.Categories.Add(org);
                context.SaveChanges();
            }
            //Create Distributor
            using(var context = new InventoryContext()) {
                Distributor distributor = new Distributor();
                distributor.Name = "SVC";
                distributor.Description = "Parent Company";

                context.Distributors.Add(distributor);
                context.SaveChanges();
            }
        }
    }

    public class GenerateTables {
        //private static readonly string rootFolder = @"E:\Software Development\Databases\Import Legacy\";
        private static readonly string textFile = @"E:\Software Development\Databases\Import Legacy\Legacy.txt";
        private static string[] Headers = {"Name","Package Type","CustumPartNumber","LegacyPartNumber","Organization",
                                            "Warehouse","Received","Lot Number","Supplier Po","TimeStamp","ValidFrom",
                                            "ValidUntil","Distributer","Unit Cost","MinOrder","LeadTime","Name - Rank",
                                            "Wavelength","Power","Voltage","Quantity","MinQuantity","SafeQuantity" };

        public static int[] PartIndexes = { 0, 1, 2, 3, 4, 5 };
        public static int[] LotIndexes = { 6, 7, 8 };
        public static int[] LotCostIndexes = { 9, 10, 11, 12, 13, 14, 15 };
        public static int[] PartInstanceIndexes = { 16, 17, 18, 19, 20, 21, 22, 23 };

        private InventoryContext _context;
        private Warehouse _warehouse;
        private List<ProductType> _productTypes=new List<ProductType>();
        private Organization _organization;
        private Distributor _distributor;
        private Manufacturer _manufacturer;



        public List<string> uniqueNames;
        public List<string[]> uniqueNameLots;
        public Dictionary<string, IEnumerable<string[]>> partLots;
        public Dictionary<string, IEnumerable<string[]>> lotRanks;
        public List<string> uniqueLots;
        public List<string> lotcost;
        public List<string> ranks;
        public List<string[]> rawData;
        public List<string> uniqueTypes;
        public List<string> Lots;

        public List<Product> Products;

        public List<Cost> Cost;
        public List<ProductInstance> Ranks;
        public Organization organizationCat;
        public Warehouse warehouse;
        public List<ProductType> productTypes;


        public GenerateTables() {
            this._context = new InventoryContext();
            this.uniqueNameLots = new List<string[]>();
            this.rawData = new List<string[]>();
            this.uniqueNames = new List<string>();
            this.uniqueLots = new List<string>();
            this.ranks = new List<string>();
            this.uniqueTypes = new List<string>();

            this.Products = new List<Product>();
            this.Lots = new List<string>();
            this.Cost = new List<Cost>();
            this.Ranks = new List<ProductInstance>();

            this.partLots = new Dictionary<string, IEnumerable<string[]>>();
            this.Load();
        }

        private void Load() {
            this._context.Locations.Load();
            this._context.Categories.Load();
            this._context.InventoryItems.Load();
            this._context.Instances.Load();
            this.GetDefaults();
        }

        private void GetDefaults() {
            this._warehouse = this._context.Locations.OfType<Warehouse>().FirstOrDefault(x => x.Name == "Products");
            this._organization = this._context.Categories.OfType<Organization>().FirstOrDefault(x => x.Name == "Products");
            this._productTypes = this._context.Categories.OfType<ProductType>().ToList();
            this._distributor = this._context.Distributors.FirstOrDefault(x => x.Name == "SVC");
            this._manufacturer = this._context.Manufacturers.FirstOrDefault(x => x.Name == "SVC");
        }

        public bool ReadFromFile() {

            if(File.Exists(textFile)) {
                try {
                   var lines=File.ReadAllLines(textFile);
                    List<string> names = new List<string>();
                    List<string> lots = new List<string>();
                    List<string> prices = new List<string>();
                    List<string> packageTypes = new List<string>();
                    List<string[]> nameLots = new List<string[]>();
                    foreach(var line in lines) {
                        
                        var row = line.Split('\t');
                        string[] nameLot = new string[3];
                        nameLot[0] = row[0].Trim();
                        nameLot[1] = row[7].Trim();
                        nameLot[2] = row[8].Trim();
                        nameLots.Add(nameLot);

                        names.Add(row[0]);
                        packageTypes.Add(row[1]);

                        var lotString= string.Concat("(",row[7], ",", row[8],")");
                        lots.Add(lotString);
                        this.rawData.Add(row);
                    }
                    this.uniqueNameLots = nameLots.Distinct(new CompareNameLots()).ToList();
                    this.uniqueNames = names.Distinct().ToList();
                    this.uniqueTypes = packageTypes.Distinct().ToList();
                    this.Lots = lots.Distinct().ToList();
                    foreach(var name in uniqueNames) {
                        this.partLots.Add(name, this.uniqueNameLots.Where(x => x[0] == name));
                    }
                    foreach(var lot in this.Lots) {

                    }
                    return true;
                } catch {
                    return false;
                }
            } else {
                return false;
            }
        }

        public void GeneratePackageTypes() {
            foreach(var type in this.uniqueTypes) {
                var pType = this._productTypes.FirstOrDefault(x => x.Name == type);
                if(pType == null) {
                    ProductType newType = new ProductType();
                    newType.Name = type;
                    this._context.Categories.Add(newType);
                }
            }
            this._context.SaveChanges();
        }

        public void CreatePartLots() {
            foreach(var item in this.partLots) {
                //Product product = new Product(item.Key, "");
                var product = this._context.InventoryItems.Create<Product>();
                product.Name = item.Key;
                product.Warehouse = this._warehouse;
                product.Organization = this._organization;
                product.Manufacturers.Add(this._manufacturer);
                this._context.InventoryItems.Add(product);
                Console.WriteLine("Product Created: {0}, Creating Lot",item.Key);
                foreach(var lot in item.Value) {
                    var newLot = this._context.Lots.Create();
                    newLot.LotNumber = lot[1];
                    newLot.SupplierPoNumber = lot[2];
                    newLot.Product = product;
                    this._context.Lots.Add(newLot);
                    Console.WriteLine("Lot Created: Lot: {0} PO: {1}", lot[1], lot[2]);
                    var ranks = this.rawData.Where(row => row[7] == lot[1] && row[8] == lot[2]);
                    bool priceMade = false;
                    foreach(var rank in ranks) {
                        var pstring = rank[1];
                        var pType= this._context.Categories.OfType<ProductType>().FirstOrDefault(type=>type.Name==pstring);
                        if(pType != null) {
                            product.ProductType = pType;
                        }
                        product.CustomPartName=rank[2];
                        product.LegacyName = rank[3];
                        if(!priceMade) {
                            newLot.Recieved = DateTime.Parse(rank[6]);
                            var cost = this._context.Rates.Create<Cost>();
                            cost.Distributor = this._distributor;
                            cost.TimeStamp = DateTime.Now;
                            cost.Lot = newLot;
                            cost.DistributorName = this._distributor.Name;
                            priceMade = true;
                            if(!string.IsNullOrEmpty(rank[13])) {
                                cost.Amount = Convert.ToDouble(rank[13]);
                            } else {
                                cost.Amount = 0;
                            }
                            this._context.Rates.Add(cost);
                        }

                        //ProductInstance pRank = new ProductInstance(newLot);
                        var pRank = this._context.Instances.Create<ProductInstance>(); 
                        pRank.Name = rank[16];
                        pRank.Wavelength = rank[17];
                        pRank.Power = rank[18];
                        pRank.Voltage = rank[19];
                        pRank.Quantity =Convert.ToInt32(rank[20]);
                        pRank.InventoryItem = product;
                        pRank.Lot = newLot;
                        this._context.Instances.Add(pRank);
                        Console.WriteLine("Rank Created: {0}",pRank.Name);

                    }
                }
                this._context.SaveChanges();
                //Console.WriteLine("Database SHould have new part with ranks!  Press any Key");
            }
        }
    }

    public class GenerateTransactions {
        private static readonly string textFile = @"E:\Software Development\Databases\Import Legacy\Transactions.txt";
        private static string[] Headers = {"Name","Package Type","CustumPartNumber","LegacyPartNumber","Organization",
                                            "Warehouse","Received","Lot Number","Supplier Po","TimeStamp","ValidFrom",
                                            "ValidUntil","Distributer","Unit Cost","MinOrder","LeadTime","Name - Rank",
                                            "Wavelength","Power","Voltage","Quantity","MinQuantity","SafeQuantity" };
        public List<string[]> rawData;
        public InventoryContext _context;
        public GenerateTransactions() {
            this.rawData = new List<string[]>();
            this._context = new InventoryContext();
            this._context.InventoryItems.Load();
            this._context.Lots.Include(e=>e.ProductInstances).Load();
            this._context.Locations.Load();
            this._context.Categories.Load();
        }

        public bool ReadFromFile() {
            if(File.Exists(textFile)) {
                try {
                    var lines = File.ReadAllLines(textFile);

                    foreach(var line in lines) {
                        var row = line.Split('\t');
                        this.rawData.Add(row);
                    }
                    return true;
                } catch {
                    return false;
                }
            } else {
                return false;
            }
        }

        public void Generate() {
            foreach(var row in rawData) {
                var partName = row[2].Trim();
                var lotNumber = row[3].Trim();
                var poNumber = row[4].Trim();
                var rankName = row[5].Trim();
                var product = this._context.InventoryItems.OfType<Product>().FirstOrDefault(x=>x.Name==partName);
                var lot = this._context.Lots.Include(e=>e.ProductInstances).FirstOrDefault(x=>x.LotNumber==lotNumber && x.SupplierPoNumber==poNumber);
                var consumer = this._context.Locations.OfType<Consumer>().FirstOrDefault(x => x.Name == "Customer");
                var warehouse = this._context.Locations.OfType<Warehouse>().FirstOrDefault(x => x.Name == "Products");
                var session = this._context.Sessions.FirstOrDefault(x => x.Id == 1);
                
                if(product!=null && lot != null && consumer!=null && warehouse!=null) {
                    var tran = this._context.Transactions.Create<ProductTransaction>();
                    //tran.Lot = lot;
                    tran.ProductName = product.Name;
                    var rank = lot.ProductInstances.FirstOrDefault(x => x.Name == rankName);
                    if(rank != null) {
                        tran.TimeStamp = DateTime.Parse(row[0]);
                        tran.Instance = rank;
                        tran.Quantity = Convert.ToInt32(row[6]);
                        tran.BuyerPoNumber = row[7];
                        tran.RMA_Number = row[8];
                        tran.Location = row[9] == "Customer" ? (Location)consumer : (Location)warehouse;
                        tran.InventoryAction = row[1] == "Outgoing" ? InventoryAction.OUTGOING : InventoryAction.RETURNING;
                        tran.IsReturning = false;
                        tran.Session = session;
                        this._context.Transactions.Add(tran);
                        this._context.SaveChanges();
                        //Console.WriteLine("Created! Part: {0} Lot: ({1},{2})",tran.ProductName,tran.LotNumber,tran.SupplierPoNumber);
                        //Console.ReadKey();
                    } else {
                        Console.WriteLine("Rank: {0} not found in Lot.  Press Any Key To Continue");
                        //Console.ReadKey();
                    }
                } else {
                    Console.WriteLine("Product: {0} Lot:({1},{2}) Missing, Press Any Key To Continue", partName,lotNumber,poNumber);
                    //Console.ReadKey();
                }
            }
        }
    }

    public static class SplitArray {
        public static void Split<T>(T[] source, int index, out T[] first, out T[] last) {
            int len2 = source.Length - index;
            first = new T[index];
            last = new T[len2];
            Array.Copy(source, 0, first, 0, index);
            Array.Copy(source, index, last, 0, len2);
        }
    }

    public class ComparePartName : IEqualityComparer<string[]> {

        public bool Equals(string[] x, string[] y) {
            return string.Equals(x[0], y[0]);
        }

        public int GetHashCode(string[] obj) {
            return obj[0].GetHashCode();
        }
    }

    public class CompareInstance : IEqualityComparer<Instance> {
        public bool Equals(Instance x, Instance y) {
            return string.Equals(x.Name, y.Name);
        }

        public int GetHashCode(Instance obj) {
            return obj.Name.GetHashCode();
        }
    }

    public class CompareNameLots : IEqualityComparer<string[]> {

        public bool Equals(string[] x, string[] y) {
            string keyX = string.Concat(x[0], x[1], x[2]);
            string keyY = string.Concat(y[0], y[1], y[2]);
            return string.Equals(keyX, keyY);
        }

        public int GetHashCode(string[] obj) {
            var val = string.Concat(obj[0], obj[1], obj[2]);
            return val.GetHashCode();
        }
    }

    public class CompareLotKey : IEqualityComparer<string[]> {
        bool IEqualityComparer<string[]>.Equals(string[] x, string[] y) => throw new NotImplementedException();
        int IEqualityComparer<string[]>.GetHashCode(string[] obj) => throw new NotImplementedException();
    }

}
