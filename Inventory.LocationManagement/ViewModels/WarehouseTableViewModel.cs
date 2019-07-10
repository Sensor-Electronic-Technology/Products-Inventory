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
using System.Text;

namespace Inventory.LocationManagement.ViewModels {
    public class WarehouseTableViewModel : InventoryViewModelBase {

        private LocationDataManager _locationManager;
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("WarehouseTableNotification"); } }
        public IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("WarehouseTableDispatcher"); } }

        private List<Warehouse> _warehouses = new List<Warehouse>();
        private Warehouse _selectedWarehouse = new Warehouse();

        public Prism.Commands.DelegateCommand WarehouseSelectedCommand { get; private set; }
        public Prism.Commands.DelegateCommand DeleteWarehouseCommand { get; private set; }
        public Prism.Commands.DelegateCommand LoadedCommand { get; private set; }
        public Prism.Commands.DelegateCommand NewWarehouseCommand { get; private set; }


        public WarehouseTableViewModel(LocationDataManager locationDataManager, IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._locationManager = locationDataManager;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.WarehouseSelectedCommand = new PrismCommands.DelegateCommand(this.WarehouseSelectedHandler);
            this.DeleteWarehouseCommand = new PrismCommands.DelegateCommand(this.DeleteWarehouseHandler);
            this.LoadedCommand = new PrismCommands.DelegateCommand(this.LoadedAsyncHandler);
            this.NewWarehouseCommand = new PrismCommands.DelegateCommand(this.NewWarehouseHandler);

            this._eventAggregator.GetEvent<WarehouseSaveEvent>().Subscribe(this.SaveWarehouseHandler);
            this._eventAggregator.GetEvent<WarehouseUpdateEvent>().Subscribe(this.UpdateWarehouseHandler);
            this._eventAggregator.GetEvent<WarehouseDiscardEvent>().Subscribe(this.DiscardWarehouseChangesHandler);

            this.PopulateAsync();
        }

        public List<Warehouse> Warehouses {
            get => this._warehouses;
            set => SetProperty(ref this._warehouses, value, "Warehouses");
        }

        public Warehouse SelectedWarehouse {
            get => this._selectedWarehouse;
            set => SetProperty(ref this._selectedWarehouse, value, "SelectedWarehouse");
        }

        public override bool KeepAlive {
            get => false;
        }

        private void WarehouseSelectedHandler() {
            if(this.SelectedWarehouse != null) {
                NavigationParameters param = new NavigationParameters();
                param.Add("Warehouse", this.SelectedWarehouse);
                param.Add("IsNew", false);
                this._regionManager.RequestNavigate(Regions.LocationDetailsRegion, AppViews.WarehouseDetailsView, param);
            }
        }

        private void NewWarehouseHandler() {
            NavigationParameters param = new NavigationParameters();
            Warehouse newWarehouse = new Warehouse();
            param.Add("Warehouse", newWarehouse);
            param.Add("IsNew", true);
            this._regionManager.RequestNavigate(Regions.LocationDetailsRegion, AppViews.WarehouseDetailsView, param);
        }

        private void DeleteWarehouseHandler() {
            if(this.SelectedWarehouse != null) {
                StringBuilder builder = new StringBuilder();
                var responce = this.MessageBoxService.ShowMessage("You are about to delete Package Type: "
                    + Environment.NewLine
                    + this.SelectedWarehouse.Name
                    + "Continue?"
                    + "WARNING:  This cannot be undone! "
                    , "Warning"
                    , MessageButton.YesNo
                    , MessageIcon.Warning
                    , MessageResult.No);
                if(responce == MessageResult.Yes) {
                    var deletedWarehouse = this._locationManager.WarehouseOperations.Delete(this.SelectedWarehouse);
                    if(deletedWarehouse != null) {
                        this._locationManager.Commit();
                        this.PopulateAsync();
                        this.DispatcherService.BeginInvoke(new Action(() => {
                            this.MessageBoxService.ShowMessage("Warehouse: " + deletedWarehouse.Name + " Deleted", "Deleted", MessageButton.OK, MessageIcon.Exclamation);
                        }));
                    } else {
                        this._locationManager.UndoChanges();
                        this.PopulateAsync();
                        this.DispatcherService.BeginInvoke(new Action(() => {
                            this.MessageBoxService.ShowMessage(" Error Deleting Warehouse: " + this.SelectedWarehouse.Name
                                + Environment.NewLine
                                + "Please Contact Administrator", "Error", MessageButton.OK, MessageIcon.Error);
                        }));
                    }
                } else {
                    this._locationManager.UndoChanges();
                    this.PopulateAsync();
                    this.DispatcherService.BeginInvoke(new Action(() => {
                        this.MessageBoxService.ShowMessage("No Changes Made, Reloading! ", "Action Canceled", MessageButton.OK, MessageIcon.Error);
                    }));
                }
            }
        }

        private void UpdateWarehouseHandler(Warehouse warehouse) {
            if(warehouse != null) {
                var toUpdate = this._locationManager.WarehouseOperations.Update(warehouse);
                if(toUpdate != null) {
                    this._locationManager.Commit();
                    this.PopulateAsync();
                    this.DispatcherService.BeginInvoke(new Action(() => {
                        this.MessageBoxService.ShowMessage("Warehouse: " + toUpdate.Name + " saved!", "Saved", MessageButton.OK, MessageIcon.Information);
                    }));
                } else {
                    this._locationManager.UndoChanges();
                    this.PopulateAsync();

                    this.DispatcherService.BeginInvoke(new Action(() => {
                        this.MessageBoxService.ShowMessage(" Error Updating Warehouse: " + warehouse.Name
                            + Environment.NewLine
                            + "Please Contact Administrator", "Error", MessageButton.OK, MessageIcon.Error);
                    }));
                }
            }
        }

        private void SaveWarehouseHandler(Warehouse warehouse) {
            if(warehouse != null) {
                var toAdd = this._locationManager.WarehouseOperations.Add(warehouse);
                if(toAdd != null) {
                    this._locationManager.Commit();
                    this.PopulateAsync();
                    this.DispatcherService.BeginInvoke(new Action(() => {
                        this.MessageBoxService.ShowMessage("Warehouse: " + toAdd.Name + " saved!",
                            "Saved", MessageButton.OK, MessageIcon.Information);
                    }));
                } else {
                    this._locationManager.UndoChanges();
                    this.PopulateAsync();
                    this.DispatcherService.BeginInvoke(new Action(() => {
                        this.MessageBoxService.ShowMessage(" Error Adding Warehouse: " + warehouse.Name
                            + Environment.NewLine
                            + "Please Contact Administrator", "Error", MessageButton.OK, MessageIcon.Error);
                    }));
                }
            }
        }

        private void DiscardWarehouseChangesHandler() {
            this._locationManager.UndoChanges();
            this._regionManager.Regions[Regions.LocationDetailsRegion].RemoveAll();
            this.PopulateAsync();
            this.DispatcherService.BeginInvoke(new Action(() => {
                this.MessageBoxService.ShowMessage("Changes Discared,Reloading", "Discarded", MessageButton.OK, MessageIcon.Information);
            }));
        }

        private void Populate() {
            this.Warehouses = this._locationManager.WarehouseProvider.GetEntityList().ToList();
        }

        private async void PopulateAsync() {
            this.Warehouses = (await this._locationManager.WarehouseProvider.GetEntityListAsync()).ToList();
        }

        private void LoadedHandler() {
            this._locationManager.WarehouseProvider.LoadData();
        }

        private async void LoadedAsyncHandler() {
            await this._locationManager.WarehouseProvider.LoadDataAsync();
        }
    }
}