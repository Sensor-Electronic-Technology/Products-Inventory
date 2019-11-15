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
using System.Data;

namespace Inventory.Reporting.ViewModels {
    public class ReportsTransactionSummaryViewModel : InventoryViewModelBase {
        private readonly InventoryContext _context;
        public IExportService ExportServiceItemizedTransactions { get => ServiceContainer.GetService<IExportService>("ItemizedTransactionsExportService"); }
        public IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("ReportTransactionDispatcher"); }

        private ObservableCollection<ProductTransaction> _transactions = new ObservableCollection<ProductTransaction>();
        private DateTime _start, _stop;
        private bool _isLoading = false;

        public AsyncCommand CollectTransactionsCommand { get; set; }
        public AsyncCommand<ExportFormat> ExportTransactionsCommand { get; set; }

        public ReportsTransactionSummaryViewModel(InventoryContext context) {
            this.Start = DateTime.Now;
            this.Stop = DateTime.Now;
            this._context = context;
            this.CollectTransactionsCommand = new AsyncCommand(this.CollectTransactionsHandler);
            this.ExportTransactionsCommand = new AsyncCommand<ExportFormat>(this.ExportTransactionsHandler);
        }

        public override bool KeepAlive {
            get => true;
        }

        public ObservableCollection<ProductTransaction> Transactions{ 
            get => this._transactions;
            set => SetProperty(ref this._transactions, value, "Transactions");
        }

        public DateTime Start {
            get => this._start;
            set => SetProperty(ref this._start, value, "Start");
        }

        public DateTime Stop {
            get => this._stop;
            set => SetProperty(ref this._stop, value, "Stop");
        }

        public bool IsLoading {
            get => this._isLoading;
            set => SetProperty(ref this._isLoading, value, "IsLoading");
        }

        private async Task CollectTransactionsHandler() {
            this.DispatcherService.BeginInvoke(() => this.IsLoading = true);
            var start = new DateTime(this._start.Year, this._start.Month, this._start.Day, 0, 0, 0, DateTimeKind.Local);
            var stop = new DateTime(this._stop.Year, this._stop.Month, this._stop.Day, 23, 59, 59, DateTimeKind.Local);

            var list = await this._context.Transactions.OfType<ProductTransaction>()
                .Include(e => e.Instance.InventoryItem)
                .Include(e => e.Location).Where(e => e.TimeStamp >= start && e.TimeStamp <= stop)
                .ToListAsync();

            this.Transactions.Clear();
            this.Transactions.AddRange(list);
            this.DispatcherService.BeginInvoke(() => this.IsLoading = false);
        }


        private async Task ExportTransactionsHandler(ExportFormat format) {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
                    using (FileStream file = File.Create(path)) {
                        this.ExportServiceItemizedTransactions.Export(file, format, new DevExpress.XtraPrinting.XlsxExportOptionsEx() {
                            ExportType = DevExpress.Export.ExportType.WYSIWYG
                        });
                    }
                    Process.Start(path);
                });
            });
        }

    }
}