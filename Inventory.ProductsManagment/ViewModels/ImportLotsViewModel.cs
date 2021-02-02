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
using System.Collections.ObjectModel;
using Inventory.Common.BuisnessLayer;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using DevExpress.Xpf.Spreadsheet;
using DevExpress.Spreadsheet;
using System.Drawing;
using Inventory.ProductsManagment.Local;

namespace Inventory.ProductsManagment.ViewModels {
    public class ImportLotsViewModel : InventoryViewModelNavigationBase {

        public override bool KeepAlive => false;
        private ProductDataManager _dataManager;
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private Worksheet _worksheetSource;
        private List<ImportLotData> _data;

        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("ImportMessageService"); } }
        public IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("ImportDispatcher"); } }
        public IOpenFileDialogService OpenFileDialogService { get { return ServiceContainer.GetService<IOpenFileDialogService>("OpenFileDialog"); } }

        public AsyncCommand ImportLotCommand { get; private set; }
        public AsyncCommand<Worksheet> SpreadsheetLoadedCommand { get; private set; }
        public AsyncCommand ProcessLotsCommand { get; private set; }

        public ImportLotsViewModel(ProductDataManager dataManager, IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._dataManager = dataManager;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this.SpreadsheetLoadedCommand = new AsyncCommand<Worksheet>(this.SpreadsheetLoadedHandler);
            this.ProcessLotsCommand = new AsyncCommand(this.ProcessLotsHandler);
        }

        public Worksheet Worksheet {
            get => this._worksheetSource;
            set => SetProperty(ref this._worksheetSource, value);
        }

        private async Task SpreadsheetLoadedHandler(Worksheet activesheet) {
            await this.DispatcherService.BeginInvoke(() =>{
                this.Worksheet = activesheet;
                this.Worksheet[0,0].Value = "Lot Date";
                this.Worksheet[0, 1].Value = "Part";
                this.Worksheet[0, 2].Value = "Lot";
                this.Worksheet[0, 3].Value = "Rank";
                this.Worksheet[0, 4].Value = "Quantity";
                this.Worksheet[0, 5].Value = "Unit Cost";
                this.Worksheet[0, 6].Value = "Supplier Po";
                this.Worksheet["A1:G1"].FillColor = Color.LightBlue;

            });
        }

        private async Task ProcessLotsHandler() {
            await this.DispatcherService.BeginInvoke(() => {
                this._data = new List<ImportLotData>();
                var range=this.Worksheet.GetUsedRange();
                int rowCount = range.RowCount;
                int colCount = range.ColumnCount;
                //BindableList<ImportLotData> range.GetDataSource(new RangeDataSourceOptions() { UseFirstRowAsHeader = true });
                for (int i = 2; i <= rowCount; i++) {
                    ImportLotData lotData = new ImportLotData();
                    lotData.LotDate = range[i, 0].Value.DateTimeValue;
                    lotData.ProductName = range[i, 1].Value.ToString();
                    lotData.LotNumber = range[1, 2].Value.ToString();
                    lotData.Rank = range[i, 3].Value.ToString();
                    lotData.Quantity = int.Parse(range[i, 4].Value.ToString());
                    lotData.UnitCost = double.Parse(range[i, 5].Value.ToString());
                    lotData.PoNumber = range[i, 6].Value.ToString();
                    this._data.Add(lotData);
                    this.Worksheet[i, 7].Value = "Processed";
                }
                this.MessageBoxService.ShowMessage(range.GetReferenceA1());
            });
        }



        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {

        }
    }
}