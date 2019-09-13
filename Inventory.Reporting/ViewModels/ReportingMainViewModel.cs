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
    public class ReportingMainViewModel : InventoryViewModelBase {
        private readonly object SyncRoot = new object();

        private InventoryContext _context;
        private ProductDataManager _dataManager;
        public IExportService ExportServiceTotalInventory { get => ServiceContainer.GetService<IExportService>("TotalInventoryExportService"); }
        public IExportService ExportServiceTransactions { get => ServiceContainer.GetService<IExportService>("ItemizedTransactionsExportService"); }
        public IExportService LotExportService { get => ServiceContainer.GetService<IExportService>("LotExportService"); }
        public IDispatcherService Dispatcher { get => ServiceContainer.GetService<IDispatcherService>("ReportsDispatcherService"); }

        private DateTime _startDate;
        private DateTime _stopDate;
        private DateTime _transactionStartDate;
        private DateTime _transactionStopDate;
        private bool _isLoading = false;
        private bool _isTotalsLoading = false;
        private double _total = 0;

        private double _startingQuantity = 0.00;
        private double _endingQuantity = 0.00;
        private double _startingCost = 0.00;
        private double _endingCost = 0.00;

        private ObservableCollection<Lot> _lots = new ObservableCollection<Lot>();
        private ObservableCollection<ReportDataRow> _summaryData = new ObservableCollection<ReportDataRow>();
        private ObservableCollection<TotalReportDataRow> _totals = new ObservableCollection<TotalReportDataRow>();
        private ObservableCollection<ProductTransaction> _transactions = new ObservableCollection<ProductTransaction>();

        public AsyncCommand GatherData { get; private set; }
        public AsyncCommand GatherTotalData { get; private set; }
        public AsyncCommand GatherTransactions { get; private set; }
        public AsyncCommand LoadLotsCommand { get; private set; }
        public DelegateCommand<ExportFormat> ExportTotalInventoryCommand { get; private set; }
        public DelegateCommand<ExportFormat> ExportTransactionsCommand { get; private set; }
        public DelegateCommand<ExportFormat> ExportLotsCommand { get; private set; }

        public ReportingMainViewModel(InventoryContext context,ProductDataManager dataManager) {
            this._dataManager = dataManager;
            this._context = context;
            this.GatherData = new AsyncCommand(this.CollectDataHandler);
            this.GatherTotalData = new AsyncCommand(this.CollectTotalDataHandler);
            this.GatherTransactions = new AsyncCommand(this.CollectItemizedTransactionsHandler);
            this.LoadLotsCommand = new AsyncCommand(this.LoadLotDataHandler);
            this.ExportTotalInventoryCommand = new DelegateCommand<ExportFormat>(this.ExportTotalSummaryHandler);
            this.ExportTransactionsCommand = new DelegateCommand<ExportFormat>(this.ExportTransactionsHandler);
            this.ExportLotsCommand = new DelegateCommand<ExportFormat>(this.ExportLotsHandler);
            this.StartDate = DateTime.Now;
            this.StopDate = DateTime.Now;
            this.TransactionStartDate = DateTime.Now;
            this.TransactionStopDate = DateTime.Now;
            this.LoadData();
        }

        public DateTime StartDate {
            get => this._startDate;
            set => SetProperty(ref this._startDate, value);
        }

        public DateTime StopDate {
            get => this._stopDate;
            set => SetProperty(ref this._stopDate, value);
        }

        public DateTime TransactionStartDate {
            get => this._transactionStartDate;
            set => SetProperty(ref this._transactionStartDate, value);
        }

        public DateTime TransactionStopDate {
            get => this._transactionStopDate;
            set => SetProperty(ref this._transactionStopDate, value);
        }

        public ObservableCollection<ReportDataRow> SummaryData {
            get => this._summaryData;
            set => SetProperty(ref this._summaryData, value);
        }

        public ObservableCollection<TotalReportDataRow> ProductTotals {
            get => this._totals;
            set => SetProperty(ref this._totals, value);
        }

        public ObservableCollection<ProductTransaction> Transactions {
            get => this._transactions;
            set => SetProperty(ref this._transactions, value);
        }

        public ObservableCollection<Lot> Lots {
            get => this._lots;
            set => SetProperty(ref this._lots, value);
        }

        public double Total {
            get => this._total;
            set => SetProperty(ref this._total, value);
        }

        public bool IsLoading {
            get=>this._isLoading;
            set => SetProperty(ref this._isLoading, value);
        }

        public bool IsTotalsLoading {
            get => this._isTotalsLoading;
            set => SetProperty(ref this._isTotalsLoading, value);
        }

        public double StartingQuantity {
            get => this._startingQuantity;
            set => SetProperty(ref this._startingQuantity, value);
        }

        public double EndingQuantity {
            get => this._endingQuantity;
            set => SetProperty(ref this._endingQuantity, value);
        }

        public double StartingCost {
            get => this._startingCost;
            set => SetProperty(ref this._startingCost, value);
        }

        public double EndingCost {
            get => this._endingCost;
            set => SetProperty(ref this._endingCost, value);
        }


        public override bool KeepAlive {
            get => true;
        }

        private void ExportTransactionsHandler(ExportFormat format) {
            var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
            using(FileStream file = File.Create(path)) {
                this.ExportServiceTransactions.Export(file, format);
            }
            Process.Start(path);
        }

        private void ExportTotalSummaryHandler(ExportFormat format) {
            var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
            using(FileStream file = File.Create(path)) {
                this.ExportServiceTotalInventory.Export(file, format);
            }
            Process.Start(path);
        }

        private void ExportLotsHandler(ExportFormat format) {
            var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
            using (FileStream file = File.Create(path)) {
                this.LotExportService.Export(file, format);
            }
            Process.Start(path);
        }

        private async Task CollectDataHandler() {
            ObservableCollection<ReportDataRow> data = new ObservableCollection<ReportDataRow>();
            double endingQuantity=0, endingCost=0;
            this.IsLoading = true;
            var transactions = await this._context.Transactions.OfType<ProductTransaction>()
                .AsNoTracking()
                .Include(e => e.Instance.InventoryItem)
                .Include(e => e.Location)
                .Where(t => (t.TimeStamp >= this.StartDate && t.TimeStamp <= this.StopDate))
                .ToListAsync();

            await Task.Run(() => {
                transactions.ForEach(transaction => {
                    var rank = this._context.Instances.OfType<ProductInstance>()
                                        .AsNoTracking()
                                        .Include(e => e.Lot.Cost)
                                        .FirstOrDefault(e => e.Id == transaction.InstanceId);
                    var row = new ReportDataRow(transaction, rank.Lot);
                    //if (transaction.InventoryAction == InventoryAction.OUTGOING) {
                    //     temp=(transaction.Quantity * -1);
                    //} else if(transaction.InventoryAction == InventoryAction.INCOMING) {
                    //    temp= transaction.Quantity;
                    //}
                    endingQuantity += row.Transaction.Quantity;
                    endingCost += row.Cost;

                    data.Add(row);
                });
            });
            this.EndingQuantity = this.StartingQuantity+endingQuantity;
            this.EndingCost = this.StartingCost + endingCost;
            this.SummaryData = data;
            this.IsLoading = false;

            //lock (SyncRoot) {
            //    this.SummaryData = data;
            //    this.IsLoading = false;
            //}

        }

        private async Task CollectTotalDataHandler() {
            ObservableCollection<TotalReportDataRow> summary = new ObservableCollection<TotalReportDataRow>();
            double total = 0;
            this.IsTotalsLoading = true;

            var products = await this._context.InventoryItems.OfType<Product>()
                .AsNoTracking()
                .Include(e => e.Attachments)
                .Include(e => e.Lots.Select(x => x.ProductInstances))
                .Include(e => e.Lots.Select(x => x.Cost))
                .Include(e => e.Warehouse)
                .Include(e => e.ProductType)
                .Include(e => e.Organization)
                .Include(e => e.Manufacturers).ToListAsync();

            await Task.Run(() => {
                foreach(var product in products) {
                    var row = new TotalReportDataRow();
                    var pTotal = product.Lots.Sum(lot => {
                        var quantity = lot.ProductInstances.Sum(rank => rank.Quantity);
                        if(lot.Cost!=null) {
                            return quantity * lot.Cost.Amount;
                        } else {
                            var cost = this._context.Rates.OfType<Cost>()
                                        .Include(e => e.Lot)
                                        .FirstOrDefault(x => x.LotNumber == lot.LotNumber && x.SupplierPoNumber == lot.SupplierPoNumber);
                            if(cost != null) {
                                return quantity * cost.Amount;
                            } else {
                                return quantity * 0;
                            }
                        }
                    });
                    total += pTotal;
                    row.Product = product;
                    row.TotalCost = pTotal;
                    summary.Add(row);
                }
            });

            lock(this.SyncRoot) {
                this.ProductTotals = summary;
                this.Total = total;
                this.IsTotalsLoading = false;
            }
        }

        private async Task CollectItemizedTransactionsHandler() {
            var list=await this._context.Transactions.OfType<ProductTransaction>().Include(e => e.Instance.InventoryItem)
                .Include(e => e.Location).Where(e=>e.TimeStamp>=this.TransactionStartDate && e.TimeStamp<=this.TransactionStopDate)
                .ToListAsync();

            lock(this.SyncRoot) {
                this.Transactions.Clear();
                this.Transactions.AddRange(list);
            }
        }

        private async Task LoadLotDataHandler() {
            await Task.Run(() => {
                this.Lots = this._context.Lots.Local;
            });
        }

        private async void LoadData() {
            await this._context.Transactions.OfType<ProductTransaction>().Include(e=>e.Location).Include(e=>e.Instance.InventoryItem).LoadAsync();
            await this._context.Lots.Include(e=>e.Product).Include(e => e.Cost).Include(e => e.ProductInstances.Select(x => x.InventoryItem)).LoadAsync();

            await this._context.Instances.OfType<ProductInstance>()
                .Include(e => e.Transactions.Select(x=>x.Location))
                .Include(e=>e.Transactions.Select(x=>x.Instance.InventoryItem))
                .Include(e=>e.InventoryItem)
                .Include(e=>e.Lot.Cost)
                .LoadAsync();

            await this._context.InventoryItems.OfType<Product>()
                        .Include(e => e.Attachments)
                        .Include(e => e.Lots.Select(x => x.ProductInstances))
                        .Include(e => e.Lots.Select(x => x.Cost))
                        .Include(e => e.Warehouse)
                        .Include(e => e.ProductType)
                        .Include(e => e.Organization)
                        .Include(e => e.Manufacturers).LoadAsync();
        }
    }
}