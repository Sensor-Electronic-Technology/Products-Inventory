﻿using System;
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

        private ObservableCollection<ProductCostSnapshot> _productSnapshot = new ObservableCollection<ProductCostSnapshot>();
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

        private async Task CollectSnapshotHandler() {
            this.DispatcherService.BeginInvoke(() => this.IsLoading = true);
            await this._context.InventoryItems.OfType<Product>()
                .Include(e => e.Lots.Select(i => i.ProductInstances.Select(x => x.Transactions)))
                .Include(e => e.Lots.Select(i => i.Cost)).LoadAsync();

            var products =await this._context.InventoryItems.OfType<Product>()
                .Include(e => e.Lots.Select(i => i.ProductInstances.Select(x => x.Transactions)))
                .Include(e => e.Lots.Select(i => i.Cost)).ToListAsync();
            ObservableCollection<ProductCostSnapshot> temp = new ObservableCollection<ProductCostSnapshot>();
            await Task.Run(()=>{ 
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
                        this.ExportServiceTransactions.Export(file, format, new DevExpress.XtraPrinting.XlsxExportOptionsEx() {
                            //ExportType = DevExpress.Export.ExportType.WYSIWYG
                        });
                    }
                    Process.Start(path);
                });
            });
        }
    }
}