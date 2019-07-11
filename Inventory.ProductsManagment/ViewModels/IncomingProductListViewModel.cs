using System;
using System.Collections.Generic;
using System.Linq;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.Common.BuisnessLayer;
using Inventory.Common.ApplicationLayer;
using DevExpress.Mvvm;
using Prism.Regions;
using Prism.Events;
using PrismCommands = Prism.Commands;
using System.Threading.Tasks;
using Inventory.Common.DataLayer.EntityDataManagers;
using Inventory.Common.ApplicationLayer.Services;
using System.Text;
using Inventory.Common.DataLayer.Providers;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;

namespace Inventory.ProductsManagment.ViewModels {
    public class IncomingProductListViewModel : InventoryViewModelNavigationBase {
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private ProductDataManager _dataManager;
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("IncomingProductListNotifications"); } }
        public IDispatcherService Dispatcher { get { return ServiceContainer.GetService<IDispatcherService>("IncomingProductListDispatcher"); } }
        public IExportService ExportService { get => ServiceContainer.GetService<IExportService>(); }

        private ObservableCollection<Lot> _incomingList = new ObservableCollection<Lot>();
        private Lot _selectedLot = new Lot();
        private List<Lot> _lots = new List<Lot>();
        private List<string> rmaList = new List<string>();

        private ProductInstance _selectedRank = new ProductInstance();
        
        public PrismCommands.DelegateCommand CheckInCommand { get; private set; }
        public AsyncCommand<ExportFormat> ExportCommand { get; private set; }
        public ICommand RemoveLotFromIncomingCommand { get; private set; }

        public IncomingProductListViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, ProductDataManager dataManager) {
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this._dataManager = dataManager;
            this.RemoveLotFromIncomingCommand = new PrismCommands.DelegateCommand(this.RemoveLotFromIncomingHandler);
            this.CheckInCommand = new PrismCommands.DelegateCommand(this.CheckInHandler);
            this.ExportCommand = new AsyncCommand<ExportFormat>(this.ExportHandler);

            this._eventAggregator.GetEvent<AddToIncomingEvent>().Subscribe(this.AddToIncomingHandler);
        }

        public ObservableCollection<Lot> IncomingList {
            get => this._incomingList;
            set => SetProperty(ref this._incomingList, value, "IncomingList");
        }

        public override bool KeepAlive {
            get => true;
        }

        public ProductInstance SelectedRank {
            get => this._selectedRank;
            set => SetProperty(ref this._selectedRank, value, "SelectedRank");
        }

        public Lot SelectedLot {
            get => this._selectedLot;
            set => SetProperty(ref this._selectedLot, value, "SelectedLot");
        }

        private void AddToIncomingHandler(IncomingLotCarrier carrier) {
            if(carrier != null) { 
                var existingLot = this.IncomingList.FirstOrDefault(x => x.LotNumber == carrier.Lot.LotNumber && x.SupplierPoNumber == carrier.Lot.SupplierPoNumber);
                if(existingLot == null) {
                    this.IncomingList.Add(carrier.Lot);
                    this.rmaList.Add(carrier.RMA);
                    this._eventAggregator.GetEvent<AddToIncomingCallback>().Publish(true);
                } else {
                    this._eventAggregator.GetEvent<AddToIncomingCallback>().Publish(false);
                }
            }
        }

        private void RemoveLotFromIncomingHandler() {
            if(this.SelectedLot != null) {
                if(this.IncomingList.Remove(this.SelectedLot)) {
                    RaisePropertyChanged("IncomingList");
                } else {
                    this.MessageBoxService.ShowMessage("Error Removing Lot, Please check selection and try again", "Error", MessageButton.OK, MessageIcon.Error);
                }
            }
        }

        private void CheckInHandler() {
            var responce = this._dataManager.CheckIn(this.IncomingList.ToList(), this.rmaList);
            this.Dispatcher.BeginInvoke(new Action(() => {
                this.MessageBoxService.ShowMessage(responce.Message,"Done", MessageButton.OK, MessageIcon.Information);
            }));
            this.IncomingList.Clear();
            this._dataManager.UpdateProductTotals();
            this._eventAggregator.GetEvent<DoneIncomingListEvent>().Publish();
        }

        private async Task ExportHandler(ExportFormat format) {
            await Task.Run(() => {
                this.Dispatcher.BeginInvoke(() => {
                    var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
                    using(FileStream file = File.Create(path)) {
                        this.ExportService.Export(file, format, new DevExpress.XtraPrinting.XlsxExportOptionsEx() {
                            ExportType = DevExpress.Export.ExportType.WYSIWYG
                        });
                    }
                    System.Diagnostics.Process.Start(path);
                });
            });
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) { return true; }
        public override void OnNavigatedFrom(NavigationContext navigationContext) { }
        public override void OnNavigatedTo(NavigationContext navigationContext) { }
    }
}