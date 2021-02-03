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
using System.ComponentModel;
using Inventory.Common.DataLayer;

namespace Inventory.ProductsManagment.ViewModels {
    public class ImportLotsViewModel : InventoryViewModelNavigationBase {

        public override bool KeepAlive => false;
        private bool _outgoingInProgress;
        private ProductDataManager _dataManager;
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private Worksheet _worksheetSource;
        private List<ImportLotData> _data;
        public WorksheetDataBindingCollection _bindingCollection;
        private bool incomingInProgress = false;

        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("ImportMessageService"); } }
        public IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("ImportDispatcher"); } }
        public IOpenFileDialogService OpenFileDialogService { get { return ServiceContainer.GetService<IOpenFileDialogService>("OpenFileDialog"); } }

        public AsyncCommand ImportLotCommand { get; private set; }
        public AsyncCommand<Worksheet> SpreadsheetLoadedCommand { get; private set; }
        public AsyncCommand ProcessLotsCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }
        public PrismCommands.DelegateCommand DoneCommand { get; private set; }
        public PrismCommands.DelegateCommand CancelCommand { get; private set; }

        public ImportLotsViewModel(ProductDataManager dataManager, IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._dataManager = dataManager;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this.SpreadsheetLoadedCommand = new AsyncCommand<Worksheet>(this.SpreadsheetLoadedHandler);
            this.ProcessLotsCommand = new AsyncCommand(this.ProcessLotsHandler);
            this.InitializeCommand = new AsyncCommand(this.LoadAsync);
            this.DoneCommand = new PrismCommands.DelegateCommand(this.FinishedHandler,()=>!this.incomingInProgress);
            this.CancelCommand = new PrismCommands.DelegateCommand(this.CancelHandler, () => !this.incomingInProgress);
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
                this.Worksheet[0, 7].Value = "Buyer Po";
                this.Worksheet[0, 8].Value = "Status";
                this.Worksheet.Columns[6].NumberFormat = "@";
                this.Worksheet.Columns[7].NumberFormat = "@";
                this.Worksheet["A1:I1"].FillColor = Color.LightBlue;

            });
        }

        private async Task ProcessLotsHandler() {
            //NavigationParameters parameters = new NavigationParameters();
            //this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.OutgoingProductListView);
            //this._eventAggregator.GetEvent<BatchImportRunning>().Publish();
            this.incomingInProgress = true;
            this._data = new List<ImportLotData>();
            var range=this.Worksheet.GetUsedRange();
            int rowCount = range.RowCount;
            int colCount = range.ColumnCount;
                
            //range.GetDataSource(new RangeDataSourceOptions() { UseFirstRowAsHeader = true });
            for (int i = 1; i <= rowCount-1; i++) {
                ImportLotData lotData = new ImportLotData();
                lotData.LotDate = range[i, 0].Value.DateTimeValue;
                lotData.ProductName = range[i, 1].Value.TextValue;
                lotData.LotNumber = range[i, 2].Value.TextValue;
                lotData.Rank = range[i, 3].Value.TextValue;
                lotData.Quantity = (int)range[i, 4].Value.NumericValue;
                lotData.UnitCost = range[i, 5].Value.NumericValue;
                lotData.PoNumber = range[i, 6].Value.TextValue;
                lotData.BuyerPo = range[i, 7].Value.TextValue;

                var response = await this._dataManager.CheckOutSingle(lotData);

                lotData.Success = response.Success;
                lotData.Status = response.Message;
               
                range[i, 8].Value = lotData.Status;

                //if(lotData.Success && lotData.BuyerPo!="") {
                //    await this.AddToOutgoing(lotData);
                //}
                //this._data.Add(lotData);
            }
            this.incomingInProgress = false;
            this.MessageBoxService.ShowMessage("Import Finished, Please check status column to ensure all completed","Done",MessageButton.OK,MessageIcon.Information);
            await this._dataManager.UpdateProductTotalsAsync();
        }

        private async Task AddToOutgoing(ImportLotData lotData) {
            await Task.Run(() => {
                var productInstance = this._dataManager.RankProvider.GetEntity(e => e.Name == lotData.Rank && e.InventoryItem.Name==lotData.ProductName);
                if (productInstance != null) {
                    if (!this._outgoingInProgress) {
                        this.DispatcherService.BeginInvoke(() => {
                            NavigationParameters parameters = new NavigationParameters();
                            parameters.Add("Rank", productInstance);
                            this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.OutgoingProductListView, parameters);
                        });
                    } else {
                        this._eventAggregator.GetEvent<AddToOutgoingEvent>().Publish(productInstance);
                    }
                }
            });
        }

        private void FinishedHandler() {
            this._eventAggregator.GetEvent<DoneIncomingListEvent>().Publish();
        }

        private void CancelHandler() {
            this._eventAggregator.GetEvent<CancelIncomingListEvent>().Publish();
        }

        private async Task LoadAsync() {
            await this._dataManager.LoadAsync();
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