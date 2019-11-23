using System;
using System.Linq;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.Common.ApplicationLayer;
using DevExpress.Mvvm;
using System.Threading.Tasks;
using Inventory.Common.ApplicationLayer.Services;
using Inventory.Common.EntityLayer.Model;
using System.Collections.ObjectModel;
using Inventory.Common.BuisnessLayer;
using System.Data.Entity;
using System.IO;
using System.Diagnostics;
using System.Data;

namespace Inventory.Reporting.ViewModels {
    public class ReportsSnapshotViewModel : InventoryViewModelBase {

        public IExportService ExportServiceTransactions { get => ServiceContainer.GetService<IExportService>("SnapshotTableExportService"); }
        public IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("SnapDispatcher"); }
        private InventoryContext _context;

        private ObservableCollection<ProductSnapshot> _productSnapshot = new ObservableCollection<ProductSnapshot>();
        private DateTime _start, _stop;
        private bool _isLoading = false;

        public AsyncCommand CollectSnapshotCommand { get; set; }
        public AsyncCommand<ExportFormat> ExportTransactionsCommand { get; set; }

        public ReportsSnapshotViewModel(InventoryContext context) {
            this._context = context;
            this.Start = DateTime.Now;
            this.Stop = DateTime.Now;
            this.CollectSnapshotCommand = new AsyncCommand(this.CollectSnapshotHandler);
            this.ExportTransactionsCommand=new AsyncCommand<ExportFormat>(this.ExportSnapshotHandler);
        }

        public override bool KeepAlive {
            get => true;
        }

        public ObservableCollection<ProductSnapshot> ProductSnapshot { 
            get => this._productSnapshot;
            set => SetProperty(ref this._productSnapshot, value,"ProductSnapshot");
        }
        
        public DateTime Start { 
            get => this._start;
            set => SetProperty(ref this._start, value,"Start");
        }

        public DateTime Stop { 
            get => this._stop;
            set => SetProperty(ref this._stop, value,"Stop");
        }

        public bool IsLoading {
            get=>this._isLoading;
            set=>SetProperty(ref this._isLoading,value,"IsLoading");
        }

        private async Task CollectSnapshotHandler() {
            this.DispatcherService.BeginInvoke(() => this.IsLoading = true);
            var dStart = new DateTime(this._start.Year, this._start.Month, this._start.Day, 0, 0, 0, DateTimeKind.Local);
            var dStop = new DateTime(this._stop.Year, this._stop.Month, this._stop.Day, 23, 59, 59, DateTimeKind.Local);

            await this._context.InventoryItems.OfType<Product>()
                .Include(e => e.Lots.Select(i => i.ProductInstances.Select(x => x.Transactions.Select(l => l.Location))))
                .Include(e => e.Lots.Select(i => i.Cost)).LoadAsync();

            var products = await this._context.InventoryItems.OfType<Product>()
                .Include(e => e.Lots.Select(i => i.ProductInstances.Select(x => x.Transactions.Select(l => l.Location))))
                .Include(e => e.Lots.Select(i => i.Cost)).ToListAsync();

            ObservableCollection<ProductSnapshot> temp = new ObservableCollection<ProductSnapshot>();
            await Task.Run(() => {
                foreach (var product in products) {
                    var now = DateTime.Now;

                    var incomingTransactions = from instance in product.Instances
                                               from transaction in instance.Transactions.OfType<ProductTransaction>()
                                               where (transaction.TimeStamp >= dStart && transaction.InventoryAction == InventoryAction.INCOMING)
                                               select transaction;

                    var returningTransactions = from instance in product.Instances
                                                from transaction in instance.Transactions.OfType<ProductTransaction>()
                                                where (transaction.TimeStamp >= dStart && transaction.InventoryAction == InventoryAction.RETURNING)
                                                select transaction;

                    var outgoingTransactions = from instance in product.Instances
                                               from transaction in instance.Transactions.OfType<ProductTransaction>()
                                               where (transaction.TimeStamp >= dStart && transaction.InventoryAction == InventoryAction.OUTGOING)
                                               select transaction;

                    var returningQtyTotal = returningTransactions.Sum(e => e.Quantity);
                    var returningCostTotal = returningTransactions.Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                    var returningQtyRange = returningTransactions.Where(e => e.TimeStamp <= dStop).Sum(e => e.Quantity);
                    var returningCostRange = returningTransactions.Where(e => e.TimeStamp <= dStop).Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                    //Incoming

                    var incomingQtyTotal = incomingTransactions.Sum(e => e.Quantity);
                    var incomingCostTotal = incomingTransactions.Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                    //Incoming In Range

                    var incomingQtyRange = incomingTransactions.Where(e => e.TimeStamp <= dStop).Sum(e => e.Quantity);
                    var incomingCostRange = incomingTransactions.Where(e => e.TimeStamp <= dStop).Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                    //OutGoing

                    var consumerQty = outgoingTransactions.Where(e => e.Location.Name == "Customer").Sum(e => e.Quantity);
                    var consumerCost = outgoingTransactions.Where(e => e.Location.Name == "Customer").Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                    var internalQty = outgoingTransactions.Where(e => e.Location.Name == "Internal").Sum(e => e.Quantity);
                    var internalCost = outgoingTransactions.Where(e => e.Location.Name == "Internal").Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                    var qualityScrapQty = outgoingTransactions.Where(e => e.Location.Name == "Quality Scrap").Sum(e => e.Quantity);
                    var qualityScrapCost = outgoingTransactions.Where(e => e.Location.Name == "Quality Scrap").Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                    //Outgoing In Range

                    var consumerQtyRange = outgoingTransactions.Where(e => e.Location.Name == "Customer" && e.TimeStamp <= dStop).Sum(e => e.Quantity);
                    var consumerCostRange = outgoingTransactions.Where(e => e.Location.Name == "Customer" && e.TimeStamp <= dStop).Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                    var internalQtyRange = outgoingTransactions.Where(e => e.Location.Name == "Internal" && e.TimeStamp <= dStop).Sum(e => e.Quantity);
                    var internalCostRange = outgoingTransactions.Where(e => e.Location.Name == "Internal" && e.TimeStamp <= dStop).Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                    var qualityScrapQtyRange = outgoingTransactions.Where(e => e.Location.Name == "Quality Scrap" && e.TimeStamp <= dStop).Sum(e => e.Quantity);
                    var qualityScrapCostRange = outgoingTransactions.Where(e => e.Location.Name == "Quality Scrap" && e.TimeStamp <= dStop).Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                    var outgoingQtyTotal = outgoingTransactions.Sum(e => e.Quantity);
                    var outgingCostTotal = outgoingTransactions.Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                    var outgoingQtyRange = outgoingTransactions.Where(e => e.TimeStamp <= dStop).Sum(e => e.Quantity);
                    var outgoingCostRange = outgoingTransactions.Where(e => e.TimeStamp <= dStop).Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });


                    var current = product.Instances.Sum(e => e.Quantity);
                    var currentCost = product.Instances.OfType<ProductInstance>().Sum(e => {
                        if (e.Lot.Cost != null) {
                            return e.Quantity * e.Lot.Cost.Amount;
                        } else {
                            return 0;
                        }
                    });

                    var outgoingTotalQty = consumerQty + internalQty + qualityScrapQty;
                    var outgoingTotalCost = consumerCost + internalCost + qualityScrapCost;

                    var starting = (current - incomingQtyTotal-returningQtyTotal) + outgoingQtyTotal;
                    var startingCost = (currentCost - incomingCostTotal-returningCostTotal) + outgingCostTotal;

                    var ending = (starting + incomingQtyRange+returningQtyRange) - outgoingQtyRange;
                    var endingCost = (startingCost + incomingCostRange+returningCostRange) - outgoingCostRange;

                    ProductSnapshot snapshot = new ProductSnapshot();
                    snapshot.ProductName = product.Name;
                    snapshot.QtyStart = starting;
                    snapshot.CostStart = startingCost;
                    snapshot.QtyEnd = ending;
                    snapshot.CostEnd = endingCost;
                    snapshot.QtyCurrent = current;
                    snapshot.CostCurrent = currentCost;

                    snapshot.Return.Quantity = returningQtyRange;
                    snapshot.Return.Cost = returningCostRange;

                    snapshot.Customer.Quantity = consumerQtyRange;
                    snapshot.Customer.Cost = consumerCostRange;

                    snapshot.Internal.Quantity = internalQtyRange;
                    snapshot.Internal.Cost = internalCostRange;

                    snapshot.QualityScrap.Quantity = qualityScrapQtyRange;
                    snapshot.QualityScrap.Cost = qualityScrapCostRange;

                    snapshot.ProductIncoming.Quantity = incomingQtyRange;
                    snapshot.ProductIncoming.Cost = incomingCostRange;

                    temp.Add(snapshot);
                }
            });

            this.ProductSnapshot = temp;
            //RaisePropertyChanged("ProductSnapshot");
            this.DispatcherService.BeginInvoke(() => this.IsLoading = false);
        }

        private async Task ExportSnapshotHandler(ExportFormat format) {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
                    using (FileStream file = File.Create(path)) {
                        this.ExportServiceTransactions.Export(file, format);
                    }
                    Process.Start(path);
                });
            });
        }
    }
}