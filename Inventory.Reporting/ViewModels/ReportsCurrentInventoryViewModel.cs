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
using DevExpress.XtraRichEdit.Model;
using DevExpress.DataProcessing;

namespace Inventory.Reporting.ViewModels {
    public class ReportsCurrentInventoryViewModel : InventoryViewModelBase {
        public IExportService ExportServiceTotalInventory { get => ServiceContainer.GetService<IExportService>("TotalInventoryExportService"); }
        public IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("ReportTotalsProductDispatcher"); }

        private InventoryContext _context;
        private ObservableCollection<TotalReportDataRow> _productTotals = new ObservableCollection<TotalReportDataRow>();
        private ObservableCollection<CurrentInventoryProductV2> _currentInventory = new ObservableCollection<CurrentInventoryProductV2>();
        private bool _isLoading = false;
        private DateTime _date;

        public AsyncCommand CollectProductTotalsCommand { get; set; }
        public AsyncCommand<ExportFormat> ExportProductTotalsCommand { get; set; }

        public ReportsCurrentInventoryViewModel(InventoryContext context) {
            this._context = context;
            this.CollectProductTotalsCommand = new AsyncCommand(this.CollectAgingReport);
            this.ExportProductTotalsCommand = new AsyncCommand<ExportFormat>(this.ExportProductTotalsHandler);
            this.Date = DateTime.Now;
        }

        public override bool KeepAlive {
            get => true;
        }

        public ObservableCollection<TotalReportDataRow> ProductTotals {
            get => this._productTotals;
            set => SetProperty(ref this._productTotals, value, "ProductTotals");
        }

        public bool IsLoading {
            get => this._isLoading;
            set => SetProperty(ref this._isLoading, value, "IsLoading");
        }

        public ObservableCollection<CurrentInventoryProductV2> CurrentInventory { 
            get => this._currentInventory; 
            set => SetProperty(ref this._currentInventory,value);
        }

        public DateTime Date { 
            get => this._date; 
            set => SetProperty(ref this._date,value);
        }

        private async Task CollectAgingReport() {
            await this.DispatcherService.BeginInvoke(() => this.IsLoading = true);
            var now = DateTime.Now;
            var date = new DateTime(this._date.Year, this._date.Month, this._date.Day, 0, 0, 0, DateTimeKind.Local);
            await this._context.Lots
                .Include(e => e.ProductInstances.Select(x => x.Transactions.Select(y=>y.Location)))
                .Include(e => e.Product.Warehouse)
                .Include(e => e.Cost)
                .LoadAsync();
            var lots = await this._context.Lots
                .Include(e => e.ProductInstances.Select(x => x.Transactions.Select(y => y.Location)))
                .Include(e => e.Product.Warehouse)
                .Include(e => e.Cost).ToListAsync();
            ObservableCollection<CurrentInventoryProductV2> currentInventory = new ObservableCollection<CurrentInventoryProductV2>();
            await Task.Run(() => {
                foreach (var lot in lots) {
                    var incomingTransactions = from instance in lot.ProductInstances
                                                from transaction in instance.Transactions.OfType<ProductTransaction>()
                                                where (transaction.TimeStamp >= date && transaction.InventoryAction == InventoryAction.INCOMING)
                                                select transaction;

                    var returningTransactions = from instance in lot.ProductInstances
                                                from transaction in instance.Transactions.OfType<ProductTransaction>()
                                                where (transaction.TimeStamp >= date && transaction.InventoryAction == InventoryAction.RETURNING)
                                                select transaction;

                    var outgoingTransactions = from instance in lot.ProductInstances
                                               from transaction in instance.Transactions.OfType<ProductTransaction>()
                                                where (transaction.TimeStamp >= date && transaction.InventoryAction == InventoryAction.OUTGOING)
                                                select transaction;

                    //Returning
                    var returningQtyTotal = returningTransactions.Sum(e => e.Quantity);
                    var returningCostTotal = returningTransactions.Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                    //Incoming

                    var incomingQtyTotal = incomingTransactions.Sum(e => e.Quantity);
                    var incomingCostTotal = incomingTransactions.Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });


                    //Outgoing

                    var consumerQty = outgoingTransactions.Where(e => e.Location.Name == "Customer").Sum(e => e.Quantity);
                    var consumerCost = outgoingTransactions.Where(e => e.Location.Name == "Customer").Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                    var internalQty = outgoingTransactions.Where(e => e.Location.Name == "Internal").Sum(e => e.Quantity);
                    var internalCost = outgoingTransactions.Where(e => e.Location.Name == "Internal").Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                    var qualityScrapQty = outgoingTransactions.Where(e => e.Location.Name == "Quality Scrap").Sum(e => e.Quantity);
                    var qualityScrapCost = outgoingTransactions.Where(e => e.Location.Name == "Quality Scrap").Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                    var totalOutgoingQuantity = outgoingTransactions.Sum(e => e.Quantity);
                    var totalOutingCost = outgoingTransactions.Sum(e => { return (e.TotalCost.HasValue) ? e.TotalCost.Value : 0; });

                    CurrentInventoryProductV2 inventoryItem = new CurrentInventoryProductV2();
                    inventoryItem.LotNumber = String.Concat("[", lot.LotNumber, "],[", lot.SupplierPoNumber, "]");

                    
                    var quantity = lot.ProductInstances.Sum(rank => rank.Quantity);
                    var cost = lot.Cost.Amount;
                    var totalCost = cost * quantity;
                    inventoryItem.DateSelected = date;
                    inventoryItem.ProductName = lot.Product.Name;
                    if (lot.Product.Warehouse != null) {
                        inventoryItem.LocationName = lot.Product.Warehouse.Name;
                    }
                    

                    inventoryItem.QtyEnd = (quantity - incomingQtyTotal-returningQtyTotal) + totalOutgoingQuantity;
                    inventoryItem.CostEnd = (totalCost - incomingCostTotal-returningCostTotal) + totalOutingCost;
                    inventoryItem.QtyCurrent = quantity;
                    inventoryItem.CostCurrent = cost * quantity;
                    inventoryItem.UnitCost = cost;
                    var outgoingNoFilter = from instance in lot.ProductInstances
                                           from transaction in instance.Transactions.OfType<ProductTransaction>()
                                           where (transaction.InventoryAction == InventoryAction.OUTGOING)
                                           select transaction;

                    if (outgoingNoFilter.Count() != 0) {
                        outgoingNoFilter.OrderByDescending(e => e.TimeStamp);
                        inventoryItem.DateIn = outgoingNoFilter.First().TimeStamp;
                        inventoryItem.Age = (now - inventoryItem.DateIn).Days;
                        inventoryItem.EndAge = (date - inventoryItem.DateIn).Days;
                        if (inventoryItem.QtyEnd > 0) {
                            currentInventory.Add(inventoryItem);
                        }
                    } else {
                        if (lot.Recieved.HasValue) {
                            inventoryItem.DateIn = lot.Recieved.Value;
                            inventoryItem.Age = (now - lot.Recieved.Value).Days;
                            inventoryItem.EndAge = (date - lot.Recieved.Value).Days;
                        } else {
                            inventoryItem.Age = -1;
                            inventoryItem.EndAge = -1;
                        }
                        if (inventoryItem.QtyEnd > 0) {
                            currentInventory.Add(inventoryItem);
                        }
                    }
                }
            });
            this.CurrentInventory = currentInventory;
            await this.DispatcherService.BeginInvoke(() => this.IsLoading = false);
        }

        private async Task CollectProductTotalsHandler() {
            ObservableCollection<TotalReportDataRow> summary = new ObservableCollection<TotalReportDataRow>();
            double total = 0;
            this.IsLoading = true;
            var products = await this._context.InventoryItems.OfType<Product>()
                .AsNoTracking().Include(e => e.Lots.Select(x => x.ProductInstances))
                .Include(e => e.Lots.Select(x => x.Cost))
                .ToListAsync();
            await Task.Run(() => {
                foreach (var product in products) {
                    var row = new TotalReportDataRow();
                    var pTotal = product.Lots.Sum(lot => {
                        var quantity = lot.ProductInstances.Sum(rank => rank.Quantity);
                        if (lot.Cost != null) {
                            return quantity * lot.Cost.Amount;
                        } else {
                            var cost = this._context.Rates.OfType<Cost>()
                                        .Include(e => e.Lot)
                                        .FirstOrDefault(x => x.LotNumber == lot.LotNumber && x.SupplierPoNumber == lot.SupplierPoNumber);
                            if (cost != null) {
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
            this.ProductTotals = summary;
            this.IsLoading = false;
        }

        private async Task ExportProductTotalsHandler(ExportFormat format) {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
                    using (FileStream file = File.Create(path)) {
                        this.ExportServiceTotalInventory.Export(file, format, new DevExpress.XtraPrinting.XlsxExportOptionsEx() {
                            //ExportType = DevExpress.Export.ExportType.WYSIWYG
                        });
                    }
                    Process.Start(path);
                });
            });
        }
    }
}