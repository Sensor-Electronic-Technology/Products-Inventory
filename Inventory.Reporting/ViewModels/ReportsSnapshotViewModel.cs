using System;
using System.Collections.Generic;
using System.Linq;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.Common.ApplicationLayer;
using DevExpress.Mvvm;
using Prism.Regions;
using Prism.Events;
using PrismCommands = Prism.Commands;
using System.Threading.Tasks;
using Inventory.Common.DataLayer.EntityDataManagers;
using System.Windows.Input;
using System.Windows;
using Inventory.Common.ApplicationLayer.Services;
using Inventory.Common.EntityLayer.Model;
using System.Collections.ObjectModel;
using Inventory.Common.BuisnessLayer;
using System.Data.Entity;
using System.IO;
using System.Diagnostics;

namespace Inventory.Reporting.ViewModels {
    public class ReportsSnapshotViewModel : InventoryViewModelBase {

        public IExportService ExportServiceTransactions { get => ServiceContainer.GetService<IExportService>("SnapshotTableExportService"); }
        private InventoryContext _context;

        private ObservableCollection<ProductCostSnapshot> _productSnapshot = new ObservableCollection<ProductCostSnapshot>();
        private DateTime _start, _stop;
        private bool _isLoading = false;

        public DelegateCommand CollectSnapshotCommand { get; set; }
        public DelegateCommand<ExportFormat> ExportTransactionsCommand { get; set; }

        public ReportsSnapshotViewModel(InventoryContext context) {
            this._context = context;
            this.CollectSnapshotCommand = new DelegateCommand(this.CollectSnapshotHandler);
            this.ExportTransactionsCommand=new DelegateCommand<ExportFormat>(this.ExportSnapshotHandler);
        }

        public override bool KeepAlive {
            get => true;
        }

        public ObservableCollection<ProductCostSnapshot> ProductSnapshot { 
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

        private void CollectSnapshotHandler() {
            this.IsLoading = true;
            this._context.InventoryItems.OfType<Product>()
                .Include(e => e.Lots.Select(i => i.ProductInstances.Select(x => x.Transactions)))
                .Include(e => e.Lots.Select(i => i.Cost)).Load();

            var products =  this._context.InventoryItems.OfType<Product>()
                .Include(e => e.Lots.Select(i => i.ProductInstances.Select(x => x.Transactions)))
                .Include(e => e.Lots.Select(i => i.Cost));
            ObservableCollection<ProductCostSnapshot> temp = new ObservableCollection<ProductCostSnapshot>();
            foreach (var product in products) {
                var dStart = this._start;
                var dStop = this._stop;
                var now = DateTime.Now;
                var incomingTransactions = from instance in product.Instances
                                           from transaction in instance.Transactions.OfType<ProductTransaction>()
                                           where (transaction.TimeStamp >= dStart && transaction.InventoryAction == InventoryAction.INCOMING)
                                           select transaction;


                var outgoingTransactions = from instance in product.Instances
                                           from transaction in instance.Transactions.OfType<ProductTransaction>()
                                           where (transaction.TimeStamp >= dStart && transaction.InventoryAction == InventoryAction.OUTGOING)
                                           select transaction;

                var incomingQtyTotal = incomingTransactions.Sum(e => e.Quantity);
                var incomingCostTotal = incomingTransactions.Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                var outgoingQtyTotal = outgoingTransactions.Sum(e => e.Quantity);
                var outgingCostTotal = outgoingTransactions.Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                var incomingQtyRange = incomingTransactions.Where(e => e.TimeStamp <= dStop).Sum(e => e.Quantity);
                var incomingCostRange = incomingTransactions.Where(e => e.TimeStamp <= dStop).Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

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
                var starting = (current - incomingQtyTotal) + outgoingQtyTotal;
                var startingCost = (currentCost - incomingCostTotal) + outgingCostTotal;
                var ending = (starting + incomingQtyRange) - outgoingQtyRange;
                var endingCost = (startingCost + incomingCostRange) - outgoingCostRange;
                ProductCostSnapshot snapShot = new ProductCostSnapshot();
                snapShot.ProductName = product.Name;
                snapShot.QtyStart = starting;
                snapShot.CostStart = startingCost;
                snapShot.QtyEnd = ending;
                snapShot.CostEnd = endingCost;
                snapShot.QtyCurrent = current;
                snapShot.CostCurrent = currentCost;
                snapShot.QtyIncoming = incomingQtyRange;
                snapShot.CostIncoming = incomingCostRange;
                snapShot.QtyOutgoing = outgoingCostRange;
                snapShot.CostOutgoing = outgoingQtyRange;
                temp.Add(snapShot);
             }
            this.ProductSnapshot = temp;
            RaisePropertyChanged("ProductSnapshot");
            this.IsLoading = false;
        }

        private void ExportSnapshotHandler(ExportFormat format) {
            var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());

            using (FileStream file = File.Create(path)) {
                this.ExportServiceTransactions.Export(file, format);
            }
            Process.Start(path);

        }
    }
}