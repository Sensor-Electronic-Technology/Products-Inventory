using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;
using EntityFramework.Triggers;
using System.Linq.Expressions;
using Inventory.Common.ApplicationLayer;
using Inventory.Common.DataLayer.Providers;
using Inventory.Common.DataLayer.EntityOperations;
using Inventory.Common.BuisnessLayer;
using System.Collections.ObjectModel;

namespace Inventory.Common.DataLayer.EntityDataManagers {

    public class InventoryActionResponce {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        public InventoryActionResponce(bool success,string message) {
            this.Success = success;
            this.Message = message;
        }
    }

    public class ProductDataManager : DataManagerBase {

        private IEntityDataOperations<Product> _productDataOperations;
        private IEntityDataProvider<Product> _productDataProvider;

        private IEntityDataProvider<ProductInstance> _productInstanceProvider;
        private IEntityDataOperations<ProductInstance> _productInstanceOperations;

        private IEntityDataProvider<Lot> _lotProvider;
        private IEntityDataOperations<Lot> _lotOperations;

        private IEntityDataOperations<ProductReservation> _reservationOperations;
        private IEntityDataProvider<ProductReservation> _reservationProvider;

        private IEntityDataProvider<ProductType> _categoryProvider;

        private IEntityDataProvider<Consumer> _consumerProvider;
        private IEntityDataProvider<Warehouse> _warehouseProvider;

        private IEntityDataProvider<Attachment> _attachmentProvider;

        private IEntityDataProvider<ProductTransaction> _productTransactionProvider;

        public ProductDataManager(InventoryContext context,IUserService userService) 
            : base(context, userService) {
            this._productDataOperations = new ProductDataOperations(this._context, this._userService);
            this._productDataProvider = new ProductDataProvider(this._context, this._userService);

            this._productInstanceOperations = new ProductInstanceOperations(this._context, this._userService);
            this._productInstanceProvider = new ProductInstanceProvider(this._context, this._userService);

            this._lotOperations = new LotOperations(this._context, this._userService);
            this._lotProvider = new LotProvider(this._context, this._userService);

            this._reservationProvider = new ReservationProvider(this._context, this._userService);
            this._reservationOperations = new ReservationOperations(this._context, this._userService);

            this._categoryProvider = new ProductTypeProvider(this._context, this._userService);

            this._consumerProvider = new ConsumerProvider(this._context, this._userService);
            this._warehouseProvider = new WarehouseProvider(this._context, this._userService);

            this._productTransactionProvider = new ProductTransactionProvider(this._context, this._userService);

            this._attachmentProvider = new AttachmentProvider(this._context, this._userService);
        }

        public IEntityDataProvider<Product> ProductProvider {
            get => this._productDataProvider;
        }

        public IEntityDataOperations<Product> ProductOperations {
            get => this._productDataOperations;
        }

        public IEntityDataProvider<ProductInstance> RankProvider {
            get => this._productInstanceProvider;
        }

        public IEntityDataOperations<ProductInstance> RankOperations {
            get => this._productInstanceOperations;
        }

        public IEntityDataProvider<Lot> LotProvider {
            get => this._lotProvider;
        }

        public IEntityDataOperations<Lot> LotOperations {
            get => this._lotOperations;
        }

        public IEntityDataProvider<ProductType> CategoryProvider {
            get => this._categoryProvider;
        }

        public IEntityDataProvider<Consumer> ConsumerProvider {
            get => this._consumerProvider;
        }

        public IEntityDataProvider<Warehouse> WarehouseProvider {
            get => this._warehouseProvider;
        }

        public IEntityDataProvider<ProductTransaction> ProductTransactionProvider {
            get => this._productTransactionProvider;
        }

        public IEntityDataOperations<ProductReservation> ReservationOperations {
            get => this._reservationOperations;
        }

        public IEntityDataProvider<ProductReservation> ReservationProvider {
            get => this._reservationProvider;
        }

        public IEntityDataProvider<Attachment> AttachmentProvider { 
            get => this._attachmentProvider; 
            set => this._attachmentProvider = value; 
        }

        public async Task LoadAsync() {
            await this._productInstanceProvider.LoadDataAsync();
            await this._lotProvider.LoadDataAsync();
            await this._productDataProvider.LoadDataAsync();
            await this._consumerProvider.LoadDataAsync();
            await this._warehouseProvider.LoadDataAsync();
            await this._productTransactionProvider.LoadDataAsync();
            await this._attachmentProvider.LoadDataAsync();
        }

        public void Load() {
            this._productInstanceProvider.LoadData();
            this._lotProvider.LoadData();
            this._productDataProvider.LoadData();
            this._consumerProvider.LoadData();
            this._warehouseProvider.LoadData();
            this._productTransactionProvider.LoadData();
            this._attachmentProvider.LoadData();
        }

        public async Task<IList<ReportDataRow>> CollectReportDataAsync(DateTime start,DateTime stop) {
            ObservableCollection<ReportDataRow> data = new ObservableCollection<ReportDataRow>();
            var transactions = (await this.ProductTransactionProvider.GetEntityListAsync(t => t.TimeStamp >= start && t.TimeStamp <= stop)).ToList();
            await Task.Run(() => {
                transactions.ForEach(transaction => {
                    var rank = this._productInstanceProvider.GetEntity(e => e.Id == transaction.InstanceId);
                    data.Add(new ReportDataRow(transaction, rank.Lot));
                });
            });
            return data;
        }

        public async Task UpdateProductTotalsAsync() {
            await Task.Run(()=>{
                this.Load();
                this._context.InventoryItems.OfType<Product>().Include(e => e.Instances).Include(e=>e.Lots).ToList().ForEach(product => {
                    product.Total = product.Instances.Sum(q => q.Quantity);
                });
                this._context.SaveChanges();
            });
        }

        public void UpdateProductTotals() {
            this._context.Instances.Load();
            this._context.InventoryItems.Load();
            this._context.Instances.Load();
            this._context.InventoryItems.OfType<Product>().Include(e => e.Instances).Include(e => e.Lots).ToList().ForEach(product => {
                product.Total = product.Instances.Sum(q => q.Quantity);
            });
            this._context.SaveChanges();
        }

        public bool ValidateUser(UserAction action) {
            return this._userService.Validate(action);
        }
        
        public InventoryActionResponce UploadProductAttachment(Attachment attachment) {
            var product = this._context.InventoryItems.OfType<Product>().Include(e => e.Attachments).FirstOrDefault(e => e.Id ==attachment.InventoryItemId);
            if (product != null) {
                product.Attachments.Add(attachment);
                this._context.Attachments.Add(attachment);
                this._context.Entry<Product>(product).State = EntityState.Modified;
                try {
                    this._context.SaveChanges();
                    return new InventoryActionResponce(true, "File Saved!");
                } catch {
                    this.UndoChanges();
                    return new InventoryActionResponce(false, "Error Saving File");
                }
            } else {
                return new InventoryActionResponce(false, "Error: Product Not Found");
            }

        }

        public InventoryActionResponce DeleteProductAttachment(Attachment attachment) {
            var product = this._context.InventoryItems.OfType<Product>().Include(e => e.Attachments).FirstOrDefault(e => e.Id == attachment.InventoryItemId);
            var attachEntity = product.Attachments.FirstOrDefault(e => e.Id == attachment.Id);
            if (product != null && attachEntity != null) {
                product.Attachments.Remove(attachEntity);
                this._context.Attachments.Remove(attachEntity);
                this._context.Entry<Product>(product).State = EntityState.Modified;
                try {
                    this._context.SaveChanges();
                    return new InventoryActionResponce(true, "File Deleted!");
                } catch {
                    this.UndoChanges();
                    return new InventoryActionResponce(false, "Error: Failed to Delete File");
                }
            } else {
                return new InventoryActionResponce(false, "Error: Product and/or Attachment Not Found");
            }
        }

        public InventoryActionResponce UploadLotAttachment(Attachment attachment) {
            var lot = this._context.Lots.Include(e => e.Attachments).FirstOrDefault(e => e.LotNumber==attachment.LotNumber && e.SupplierPoNumber==attachment.SupplierPoNumber);
            if (lot != null) {
                lot.Attachments.Add(attachment);
                this._context.Attachments.Add(attachment);
                this._context.Entry<Lot>(lot).State = EntityState.Modified;
                try {
                    this._context.SaveChanges();
                    return new InventoryActionResponce(true, "File Saved!");
                } catch {
                    this.UndoChanges();
                    return new InventoryActionResponce(false, "Error Saving File");
                }
            } else {
                return new InventoryActionResponce(false, "Error: Lot Not Found");
            }

        }

        public InventoryActionResponce DeleteLotAttachment(Attachment attachment) {
            var lot  = this._context.Lots.Include(e => e.Attachments).FirstOrDefault(e => e.LotNumber == attachment.LotNumber && e.SupplierPoNumber == attachment.SupplierPoNumber);
            var attachEntity = lot.Attachments.FirstOrDefault(e => e.Id == attachment.Id);
            if (lot != null && attachEntity != null) {
                lot.Attachments.Remove(attachEntity);
                this._context.Attachments.Remove(attachEntity);
                this._context.Entry<Lot>(lot).State = EntityState.Modified;
                try {
                    this._context.SaveChanges();
                    return new InventoryActionResponce(true, "File Deleted!");
                } catch {
                    this.UndoChanges();
                    return new InventoryActionResponce(false, "Error: Failed to Delete File");
                }
            } else {
                return new InventoryActionResponce(false, "Error: Product and/or Attachment Not Found");
            }
        }

        public InventoryActionResponce UpdateProductAttachment(Attachment attachment) {
            var attachEntity = this._context.Attachments.FirstOrDefault(e => e.Id == attachment.Id);
            if (attachEntity != null) {
                this._context.Entry<Attachment>(attachEntity).State = EntityState.Modified;
                try {
                    this._context.SaveChanges();
                    return new InventoryActionResponce(true, "Attachment Updated!");
                } catch {
                    this.UndoChanges();
                    return new InventoryActionResponce(false, "Error:  Update Failed");
                }
            } else {
                return new InventoryActionResponce(false, "Error: Attachment Not Found");
            }
        }



        public InventoryActionResponce CheckIn(IList<Lot> items,IList<string> rmas) {
            StringBuilder failedBuilder = new StringBuilder();
            StringBuilder successBuilder = new StringBuilder();
            StringBuilder message = new StringBuilder();
            bool failures = false;


            var defaultDistributor = this._context.Distributors.AsNoTracking().FirstOrDefault(x => x.Name == "SVC");
            var warehouse = this._context.Locations.AsNoTracking().FirstOrDefault(x => x.Name == "Products");
            for(int i=0;i<items.Count;i++) {
                StringBuilder builder = new StringBuilder();
                string lotNumber = items[i].LotNumber;
                string po = items[i].SupplierPoNumber;
                int id = items[i].Product.Id;
                var lotExist = this._context.Lots.AsNoTracking().FirstOrDefault(x => x.SupplierPoNumber == po && x.LotNumber == lotNumber);
                var productEntity = this._context.InventoryItems.AsNoTracking().OfType<Product>().FirstOrDefault(x => x.Id == id);

                if(lotExist == null && defaultDistributor != null && productEntity != null && warehouse!=null) {
                    var newLot = this._context.Lots.Create();
                    newLot.LotNumber = items[i].LotNumber;
                    newLot.SupplierPoNumber = items[i].SupplierPoNumber;
                    newLot.Recieved = items[i].Recieved;
                    newLot.ProductId = productEntity.Id;
                    newLot.ProductName = productEntity.Name;
                    productEntity.Lots.Add(newLot);

                    this._context.Lots.Add(newLot);

                    var newCost = this._context.Rates.Create<Cost>();
                    newCost.Amount = items[i].Cost.Amount;
                    newCost.DistributorId = defaultDistributor.Id;
                    newCost.Lot = newLot;
                    newCost.TimeStamp = DateTime.Now;
                    newLot.CostId = newCost.Id;
                    this._context.Rates.Add(newCost);

                    items[i].ProductInstances.ToList().ForEach(item => {
                        var newRank = this._context.Instances.Create<ProductInstance>();
                        newRank.Name = item.Name;
                        newRank.Power = item.Power;
                        newRank.Wavelength = item.Wavelength;
                        newRank.Voltage = item.Voltage;
                        newRank.Quantity = item.Quantity;
                        newRank.LotNumber = newLot.LotNumber;
                        newRank.SupplierPoNumber = newLot.SupplierPoNumber;
                        newRank.InventoryItemId = productEntity.Id;

                        this._context.Instances.Add(newRank);

                        var transaction = this._context.Transactions.Create<ProductTransaction>();
                        transaction.InstanceId = newRank.Id;
                        transaction.LocationId = warehouse.Id;
                        transaction.TimeStamp = items[i].Recieved.HasValue ? items[i].Recieved.Value:DateTime.Now;
                        transaction.SessionId = this._userService.CurrentSession.Id;                   
                        transaction.Quantity = newRank.Quantity;
                        transaction.InventoryAction = InventoryAction.INCOMING;
                        transaction.IsReturning = false;
                        transaction.ProductName = productEntity.Name;
                        transaction.RMA_Number = rmas[i];
                        transaction.UnitCost = newLot.Cost.Amount;
                        transaction.TotalCost = transaction.UnitCost * transaction.Quantity;

                        this._context.Transactions.Add(transaction);

                        try {
                            this._context.SaveChanges();
                            successBuilder.AppendFormat("Lot: ({0},{1}) Rank: {2} => Input Quantity({3}) UnitCost: ${4}", newLot.LotNumber,newLot.SupplierPoNumber, newRank.Name,newRank.Quantity,newLot.Cost.Amount).AppendLine();
                        } catch {
                            failures = true;
                            failedBuilder.AppendFormat("Lot: ({0},{1}) or Product Id: {2} or Distributor: SVC Not Found, Internal Error See Administrator", lotExist.LotNumber, po, id);
                            this._context.UndoDbContext();
                        }
                    });
                } else {
                    failures = true;
                    if(lotExist != null) {
                        failedBuilder.AppendFormat("Lot: ({0},{1}) Exits, Please Check Lot Input and Try Again", lotNumber, po).AppendLine();
                    } else {
                        failedBuilder.AppendFormat("Product Id: {0} or Distributor: SVC or Warehouse: Products Not Found,Internal Error: Please see Administrator",id).AppendLine();
                    }
                }
            }
            if(failures) {
                message.AppendLine("Errors While Saving, Please See Below: ");
                message.Append(failedBuilder.ToString()).AppendLine();
                message.AppendLine("Items Succeeded Below: ");
                message.Append(successBuilder.ToString());
            } else {
                message.AppendLine("Succeeded");
                message.AppendLine("Items Succeeded Below: ");
                message.Append(successBuilder.ToString());
            }
            return new InventoryActionResponce(true, message.ToString());
        }

        public InventoryActionResponce Checkin(IList<ProductInstance> items,Product product,Lot lot,Cost cost,string rma=null) {
            StringBuilder builder = new StringBuilder();
            var lotentity = this._context.Lots.FirstOrDefault(x => x.SupplierPoNumber == lot.SupplierPoNumber && x.LotNumber == lot.LotNumber);
            var productEntity = this._context.InventoryItems.OfType<Product>().FirstOrDefault(x => x.Name == product.Name);
            var defaultDistributor = this._context.Distributors.FirstOrDefault(x => x.Name == "SVC");
            var warehouse = this._context.Locations.FirstOrDefault(x=>x.Name=="Products");
            if(lotentity == null && defaultDistributor!=null && productEntity!=null) {
                var newLot=this._context.Lots.Create();
                newLot.LotNumber = lot.LotNumber;
                newLot.SupplierPoNumber = lot.SupplierPoNumber;
                newLot.Recieved = lot.Recieved;
                newLot.Product = productEntity;
                this._context.Lots.Add(newLot);

                var newCost = this._context.Rates.Create<Cost>();
                newCost.Amount = cost.Amount;
                newCost.Distributor = defaultDistributor;
                newCost.Lot = newLot;
                newCost.TimeStamp = DateTime.Now;
                this._context.Rates.Add(newCost);

                items.ToList().ForEach(item => {
                    var newRank = this._context.Instances.Create<ProductInstance>();
                    newRank.Name = item.Name;
                    newRank.Power = item.Power;
                    newRank.Wavelength = item.Wavelength;
                    newRank.Voltage = item.Voltage;
                    newRank.Quantity = item.Quantity;
                    newRank.Lot = newLot;
                    newRank.InventoryItem = productEntity;
                    this._context.Instances.Add(item);

                    var transaction=this._context.Transactions.Create<ProductTransaction>();
                    transaction.Instance = newRank;
                    transaction.Location = warehouse;
                    transaction.TimeStamp = DateTime.Now;
                    transaction.Session = this._userService.CurrentSession;
                    transaction.Quantity = newRank.Quantity;
                    transaction.InventoryAction = InventoryAction.INCOMING;
                    transaction.IsReturning = false; 
                    transaction.RMA_Number=string.IsNullOrEmpty(rma) ? "":rma;
                    transaction.ProductName = newRank.InventoryItem.Name;
                    this._context.Transactions.Add(transaction);
                });
                try {
                    this.Commit();
                    return new InventoryActionResponce(true,"Success");
                } catch(Exception e) {
                    builder.AppendLine("Please try again or contact administrator with error below");
                    builder.AppendFormat("Error: {0}", e.Message).AppendLine();
                    if(e.InnerException != null) {
                        builder.AppendFormat("Inner Error: {0}", e.InnerException.Message);
                    }
                    return new InventoryActionResponce(false, builder.ToString());
                }
            }
            return new InventoryActionResponce(false, "Lot Exist,Check input and try again");
        }

        public InventoryActionResponce Checkout(IList<ProductInstance> items,Consumer selectedConsumer,DateTime timeStamp,string buyerPo=null,string rma=null) {
            StringBuilder failedBuilder = new StringBuilder();
            StringBuilder successBuilder = new StringBuilder();
            StringBuilder message = new StringBuilder();
            bool error = false;
            foreach(var item in items) {
                var rank = this._context.Instances
                    .OfType<ProductInstance>()
                    .Include(e => e.InventoryItem)
                    .Include(e => e.Transactions)
                    .Include(e=>e.Lot.Cost)
                    .FirstOrDefault(x=>x.Id==item.Id);
                var product = this._context.InventoryItems.OfType<Product>().FirstOrDefault(x => x.Name == rank.InventoryItem.Name);
                var consumer = this._context.Locations.Find(selectedConsumer.Id);
                if(rank != null && consumer!=null && product!=null) {
                    if(item.Quantity <= rank.Quantity && item.Quantity>0) {
                        rank.Quantity -= item.Quantity;
                        var transaction = this._context.Transactions.Create<ProductTransaction>();
                        transaction.TimeStamp = timeStamp;
                        transaction.InstanceId = rank.Id;
                        transaction.Quantity = item.Quantity;
                        transaction.LocationId = consumer.Id;
                        transaction.ProductName = rank.InventoryItem.Name;
                        transaction.InventoryAction = InventoryAction.OUTGOING;
                        transaction.SessionId= this._userService.CurrentSession.Id;
                        transaction.BuyerPoNumber = (!string.IsNullOrEmpty(buyerPo)) ? buyerPo : "";
                        transaction.RMA_Number = (!string.IsNullOrEmpty(rma)) ? rma : "";
                        transaction.IsReturning = false;
                        transaction.UnitCost = rank.Lot.Cost.Amount;
                        transaction.TotalCost = transaction.UnitCost * transaction.Quantity;
                        this._context.Transactions.Add(transaction);
                        successBuilder.AppendFormat("Product: {0} Rank: {1} : Removed Quantity({2}) , New Stock({3}) ", rank.InventoryItem.Name, rank.Name, item.Quantity, rank.Quantity).AppendLine();
                    } else {
                        error = true;
                        failedBuilder.AppendFormat("Product: {0} Rank: {1} : Outgoing Quantity({2}) Current Stock({3}) ",rank.InventoryItem.Name,rank.Name,item.Quantity,rank.Quantity).AppendLine();
                    }
                } else {
                    error = true;
                    failedBuilder.AppendLine("Error: Rank and Lot not found, Please check with administrator");
                }
            }

            try {
                this.Commit();
                if(error) {
                    message.AppendLine("Errors While Saving, Please See Below: ");
                    message.Append(failedBuilder.ToString()).AppendLine();
                    message.AppendLine("Items Succeeded Below: ");
                    message.Append(successBuilder.ToString());
                    return new InventoryActionResponce(true, message.ToString());
                } else {
                    message.AppendLine("Succeeded");
                    message.AppendLine("Items Succeeded Below: ");
                    message.Append(successBuilder.ToString());
                    return new InventoryActionResponce(true, message.ToString());
                }
            } catch {
                this.UndoChanges();
                message.AppendLine("Failed while saving");
                message.AppendLine(" Please try again or contact administrator");
                return new InventoryActionResponce(false, message.ToString());
            }
        }

        public InventoryActionResponce RenameLot(Lot entity, string newLotNum, string newPo) {
            if (this._userService.Validate(UserAction.Edit)) {
                var lot = this._lotOperations.Delete(entity);
                if (lot != null) {
                    var lotentity = this._context.Lots.FirstOrDefault(x => x.SupplierPoNumber == lot.SupplierPoNumber && x.LotNumber == lot.LotNumber);
                    var productEntity = this._context.InventoryItems.OfType<Product>().FirstOrDefault(x => x.Name == lot.ProductName);

                    var defaultDistributor = this._context.Distributors.FirstOrDefault(x => x.Name == "SVC");
                    var warehouse = this._context.Locations.FirstOrDefault(x => x.Name == "Products");
                    if (lotentity == null && defaultDistributor != null && productEntity != null) {
                        var newLot = this._context.Lots.Create();
                        newLot.LotNumber = newLotNum;
                        newLot.SupplierPoNumber = newPo;
                        newLot.Recieved = lot.Recieved;
                        newLot.Product = productEntity;
                        this._context.Lots.Add(newLot);

                        var newCost = this._context.Rates.Create<Cost>();
                        newCost.Amount = lot.Cost.Amount;
                        newCost.Distributor = defaultDistributor;
                        newCost.Lot = newLot;
                        newCost.TimeStamp = DateTime.Now;
                        this._context.Rates.Add(newCost);

                        lot.ProductInstances.ToList().ForEach(item => {
                            var newRank = this._context.Instances.Create<ProductInstance>();
                            newRank.Name = item.Name;
                            newRank.Power = item.Power;
                            newRank.Wavelength = item.Wavelength;
                            newRank.Voltage = item.Voltage;
                            newRank.Quantity = item.Quantity;
                            newRank.LotNumber = newLot.LotNumber;
                            newRank.SupplierPoNumber = newLot.SupplierPoNumber;
                            newRank.InventoryItem = productEntity;
                            newRank.Lot = newLot;
                            newLot.ProductInstances.Add(newRank);
                            this._context.Instances.Add(newRank);

                            item.Transactions.OfType<ProductTransaction>().ToList().ForEach(transaction => {
                                var newTransaction = this._context.Transactions.Create<ProductTransaction>();
                                newTransaction.Instance = newRank;
                                newTransaction.LocationId = transaction.LocationId;
                                newTransaction.TimeStamp = transaction.TimeStamp;
                                newTransaction.SessionId = transaction.SessionId;
                                newTransaction.Quantity = transaction.Quantity;
                                newTransaction.UnitCost = transaction.UnitCost;
                                newTransaction.TotalCost = transaction.TotalCost;
                                newTransaction.InventoryAction = transaction.InventoryAction;
                                newTransaction.IsReturning = transaction.IsReturning;
                                newTransaction.RMA_Number = transaction.RMA_Number;
                                newTransaction.ProductName = transaction.ProductName;
                                newRank.Transactions.Add(newTransaction);
                                this._context.Transactions.Add(newTransaction);
                            });
                        });
                        lot.ProductInstances.Clear();
                        lot.Cost = null;
                        lot.Product = null;
                        try {
                            this._context.SaveChanges();
                            return new InventoryActionResponce(true, "Lot Renamed");
                        } catch {
                            this.UndoChanges();
                            return new InventoryActionResponce(false, "Error Renaming Lot: Exception while saving");
                        }
                    } else {
                        this.UndoChanges();
                        return new InventoryActionResponce(false, "Error:Lot not found"); ;
                    }
                } else {
                    this.UndoChanges();
                    return new InventoryActionResponce(false, "Error: Could Not Delete Lot");
                }
            } else {
                return new InventoryActionResponce(false, "Error: You do not have permissions to perform this action");
            }
        }

        public async Task<InventoryActionResponce> RenameLotAsync(Lot entity, string newLotNum, string newPo) {
            if (this._userService.Validate(UserAction.Edit)) {
                var lot = await this._lotOperations.DeleteAsync(entity);
                if (lot != null) {
                    var lotentity = await this._context.Lots.FirstOrDefaultAsync(x => x.SupplierPoNumber == lot.SupplierPoNumber && x.LotNumber == lot.LotNumber);
                    var productEntity = await this._context.InventoryItems.OfType<Product>().FirstOrDefaultAsync(x => x.Name == lot.ProductName);

                    var defaultDistributor = await this._context.Distributors.FirstOrDefaultAsync(x => x.Name == "SVC");
                    var warehouse = await this._context.Locations.FirstOrDefaultAsync(x => x.Name == "Products");
                    if (lotentity == null && defaultDistributor != null && productEntity != null) {
                        var newLot = this._context.Lots.Create();
                        newLot.LotNumber = newLotNum;
                        newLot.SupplierPoNumber = newPo;
                        newLot.Recieved = lot.Recieved;
                        newLot.Product = productEntity;
                        this._context.Lots.Add(newLot);

                        var newCost = this._context.Rates.Create<Cost>();
                        newCost.Amount = lot.Cost.Amount;
                        newCost.Distributor = defaultDistributor;
                        newCost.Lot = newLot;
                        newCost.TimeStamp = DateTime.Now;
                        this._context.Rates.Add(newCost);
                        lot.ProductInstances.ToList().ForEach(item => {
                            var newRank = this._context.Instances.Create<ProductInstance>();
                            newRank.Name = item.Name;
                            newRank.Power = item.Power;
                            newRank.Wavelength = item.Wavelength;
                            newRank.Voltage = item.Voltage;
                            newRank.Quantity = item.Quantity;
                            newRank.LotNumber = newLot.LotNumber;
                            newRank.SupplierPoNumber = newLot.SupplierPoNumber;
                            newRank.InventoryItem = productEntity;
                            newRank.Lot = newLot;
                            newLot.ProductInstances.Add(newRank);
                            this._context.Instances.Add(newRank);

                            item.Transactions.OfType<ProductTransaction>().ToList().ForEach(transaction => {
                                var newTransaction = this._context.Transactions.Create<ProductTransaction>();
                                newTransaction.Instance = newRank;
                                newTransaction.Location = transaction.Location;
                                newTransaction.TimeStamp = transaction.TimeStamp;
                                newTransaction.SessionId = transaction.SessionId;
                                newTransaction.Quantity = transaction.Quantity;
                                newTransaction.UnitCost = transaction.UnitCost;
                                newTransaction.TotalCost = transaction.TotalCost;
                                newTransaction.InventoryAction = transaction.InventoryAction;
                                newTransaction.IsReturning = transaction.IsReturning;
                                newTransaction.RMA_Number = transaction.RMA_Number;
                                newTransaction.ProductName = transaction.ProductName;
                                newRank.Transactions.Add(newTransaction);
                                this._context.Transactions.Add(newTransaction);
                            });
                        });
                        lot.ProductInstances.Clear();
                        lot.Cost = null;
                        lot.Product = null;
                        try {
                            await this._context.SaveChangesAsync();
                            return new InventoryActionResponce(true, "Lot Renamed");
                        } catch {
                            this.UndoChanges();
                            return new InventoryActionResponce(false, "Error Renaming Lot: Exception while saving");
                        }
                    } else {
                        this.UndoChanges();
                        return new InventoryActionResponce(false, "Error:Lot not found"); ;
                    }
                } else {
                    this.UndoChanges();
                    return new InventoryActionResponce(false, "Error: Could Not Delete Lot");
                }
            } else {
                return new InventoryActionResponce(false, "Error: You do not have permissions to perform this action");
            }


        }

        public InventoryActionResponce DeleteTransaction(ProductTransaction transaction) {
            if (this._userService.Validate(UserAction.Remove)) {
                var tran = this._context.Transactions.OfType<ProductTransaction>().Include(e => e.Instance).FirstOrDefault(e => e.Id == transaction.Id);
                if (tran != null) {
                    var rank = this._context.Instances.OfType<ProductInstance>().Include(e => e.InventoryItem).FirstOrDefault(e => e.Id == tran.InstanceId);
                    if (rank != null) {
                        var product = this._context.InventoryItems.OfType<Product>().Include(e => e.Instances).FirstOrDefault(x => x.Id == rank.InventoryItemId);
                        if (product != null) {
                            //assumes was outgoing -= if incoming
                            if (transaction.InventoryAction == InventoryAction.INCOMING) {
                                rank.Quantity -= tran.Quantity;
                            } else if (transaction.InventoryAction == InventoryAction.OUTGOING) {
                                rank.Quantity += tran.Quantity;
                            }
                            rank.Transactions.Remove(tran);
                            this._context.Entry<Instance>(rank).State = EntityState.Modified;
                            this._context.Transactions.Remove(tran);
                            this._context.InventoryItems.OfType<Product>().Include(e => e.Instances).Include(e => e.Lots).ToList().ForEach(p => {
                                p.Total = p.Instances.Sum(q => q.Quantity);
                            });
                            try {
                                this._context.SaveChanges();
                                return new InventoryActionResponce(true, "Success:  Transaction deleted");
                            } catch {
                                this.UndoChanges();
                                return new InventoryActionResponce(false, "Error: Could not delete Transaction");
                            }
                        } else {
                            return new InventoryActionResponce(false, "Error: Product not found");
                        }
                    } else {
                        return new InventoryActionResponce(false, "Error: Rank not Found");
                    }
                } else {
                    return new InventoryActionResponce(false, "Error: Transaction not Found");
                }
            } else {
                return new InventoryActionResponce(false, "Error: You do not have permissions to perform this action");
            }
        }

        public async Task<InventoryActionResponce> DeleteTransactionAsync(ProductTransaction transaction) {
            if (this._userService.Validate(UserAction.Remove)) {
                var tran = await this._context.Transactions.OfType<ProductTransaction>().Include(e => e.Instance).FirstOrDefaultAsync(e => e.Id == transaction.Id);
                if (tran != null) {
                    var rank = await this._context.Instances.OfType<ProductInstance>().Include(e => e.InventoryItem).FirstOrDefaultAsync(e => e.Id == tran.InstanceId);
                    if (rank != null) {
                        var product = await this._context.InventoryItems.OfType<Product>().Include(e => e.Instances).FirstOrDefaultAsync(x => x.Id == rank.InventoryItemId);
                        if (product != null) {
                            //assumes was outgoing -= if incoming
                            if (transaction.InventoryAction == InventoryAction.INCOMING) {
                                rank.Quantity -= tran.Quantity;
                            } else if (transaction.InventoryAction == InventoryAction.OUTGOING) {
                                rank.Quantity += tran.Quantity;
                            }
                            rank.Transactions.Remove(tran);
                            this._context.Entry<Instance>(rank).State = EntityState.Modified;
                            this._context.Transactions.Remove(tran);
                            this._context.InventoryItems.OfType<Product>().Include(e => e.Instances).Include(e => e.Lots).ToList().ForEach(p => {
                                p.Total = p.Instances.Sum(q => q.Quantity);
                            });
                            try {
                                await this._context.SaveChangesAsync();
                                return new InventoryActionResponce(true, "Success:  Transaction deleted");
                            } catch {
                                this.UndoChanges();
                                return new InventoryActionResponce(false, "Error: Could not delete Transaction");
                            }
                        } else {
                            return new InventoryActionResponce(false, "Error: Product not found");
                        }
                    } else {
                        return new InventoryActionResponce(false, "Error: Rank not Found");
                    }
                } else {
                    return new InventoryActionResponce(false, "Error: Transaction not Found");
                }
            } else {
                return new InventoryActionResponce(false, "Error: You do not have permissions to perform this action");
            }
        }

        public InventoryActionResponce ReturnQuantityToInventory(ProductInstance entity, int quantity, string buyerPo = null, string rma = null) {
            var rank = this._context.Instances
                .OfType<ProductInstance>()
                .Include(e => e.Lot.Cost)
                .Include(e => e.Transactions)
                .Include(e => e.InventoryItem)
                .FirstOrDefault(e=>e.Id==entity.Id);
            var warehouse = this._context.Locations.AsNoTracking().FirstOrDefault(x => x.Name == "Products");
            if (rank != null && warehouse!=null) {
                if (quantity > 0) {
                    ProductTransaction transaction = new ProductTransaction();
                    transaction.TimeStamp = DateTime.Now;
                    transaction.UnitCost = (rank.Lot.Cost != null) ? rank.Lot.Cost.Amount : 0;
                    transaction.Quantity = quantity;
                    transaction.TotalCost = transaction.Quantity * transaction.UnitCost;
                    transaction.Location = warehouse;
                    transaction.InventoryAction = InventoryAction.RETURNING;
                    transaction.IsReturning = true;
                    transaction.RMA_Number = string.IsNullOrEmpty(rma) ? "" : rma;
                    transaction.BuyerPoNumber = (!string.IsNullOrEmpty(buyerPo)) ? buyerPo : "";
                    transaction.SessionId = this._userService.CurrentSession.Id;
                    transaction.ProductName = rank.InventoryItem.Name;
                    this._context.Transactions.Add(transaction);
                    rank.Transactions.Add(transaction);
                    rank.Quantity += quantity;
                    this._context.Entry<ProductInstance>(rank).State = EntityState.Modified;
                    this._context.InventoryItems.OfType<Product>().Include(e => e.Instances).Include(e => e.Lots).ToList().ForEach(p => {
                        p.Total = p.Instances.Sum(q => q.Quantity);
                    });
                    try {
                        this._context.SaveChanges();
                        return new InventoryActionResponce(true, "Success: Quantity Returned To Inventory");
                    } catch {
                        this.UndoChanges();
                        return new InventoryActionResponce(false, "Error: Exception while saving, all changes reverted");
                    }
                } else {
                    return new InventoryActionResponce(false, "Error: Quantity must be > 0");
                }
            } else {
                if (rank == null) {
                    return new InventoryActionResponce(false, "Error: Rank Not Found");
                } else {
                    return new InventoryActionResponce(false, "Error: Default Warehouse Not Found");
                }

            }
        }
        
        public override void UndoChanges() {
            this._context.UndoDbEntries<Product>();
            this._context.UndoDbEntries<Lot>();
            this._context.UndoDbEntries<ProductInstance>();
            this._context.UndoDbEntries<Instance>();
            this._context.UndoDbEntries<ProductTransaction>();
            this._context.UndoDbEntries<Transaction>();
            this._context.UndoDbEntries<ProductReservation>();
        }
    }

}
