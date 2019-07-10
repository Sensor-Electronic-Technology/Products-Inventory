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

    public class ConsumerTableViewModel : InventoryViewModelBase {

        private LocationDataManager _locationManager;
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("ConsumerTableNotification"); } }
        public IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("ConsumerTableDispatcher"); } }

        private List<Consumer> _consumers = new List<Consumer>();
        private Consumer _selectedConsumer = new Consumer();

        public PrismCommands.DelegateCommand ConsumerSelectedCommand { get; private set; }
        public PrismCommands.DelegateCommand DeleteConsumerCommand { get; private set; }
        public PrismCommands.DelegateCommand LoadedCommand { get; private set; }
        public PrismCommands.DelegateCommand NewConsumerCommand { get; private set; }



        public ConsumerTableViewModel(LocationDataManager locationDataManager, IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._locationManager = locationDataManager;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.ConsumerSelectedCommand = new PrismCommands.DelegateCommand(this.ConsumerSelectedHandler);
            this.DeleteConsumerCommand = new PrismCommands.DelegateCommand(this.DeleteConsumerHandler);
            this.LoadedCommand = new PrismCommands.DelegateCommand(this.LoadedAsyncHandler);
            this.NewConsumerCommand = new PrismCommands.DelegateCommand(this.NewConsumerHandler);
            this.PopulateAsync();
            this._eventAggregator.GetEvent<ConsumerUpdateEvent>().Subscribe(this.UpdateConsumerHandler);
            this._eventAggregator.GetEvent<ConsumerSaveEvent>().Subscribe(this.SaveConsumerHandler);
            this._eventAggregator.GetEvent<ConsumerDiscardEvent>().Subscribe(this.DiscardConsumerChangesHandler);
        }

        public List<Consumer> Consumers {
            get => this._consumers;
            set => SetProperty(ref this._consumers, value, "Consumers");
        }

        public Consumer SelectedConsumer {
            get => this._selectedConsumer;
            set => SetProperty(ref this._selectedConsumer, value, "SelectedConsumer");
        }

        public override bool KeepAlive {
            get => false;
        }

        private void ConsumerSelectedHandler() {
            if(this.SelectedConsumer != null) {
                NavigationParameters param = new NavigationParameters();
                param.Add("Consumer", this.SelectedConsumer);
                param.Add("IsNew", false);
                this._regionManager.RequestNavigate(Regions.LocationDetailsRegion, AppViews.ConsumerDetailsView, param);
            }
        }

        private void DeleteConsumerHandler() {
            if(this.SelectedConsumer != null) {
                StringBuilder builder = new StringBuilder();
                var responce = this.MessageBoxService.ShowMessage("You are about to delete "
                    + Environment.NewLine
                    + "Consumer: "
                    + this.SelectedConsumer.Name
                    + " Continue?"
                    + Environment.NewLine
                    + "WARNING:  This cannot be undone! "
                    , "Warning"
                    , MessageButton.YesNo
                    , MessageIcon.Warning
                    , MessageResult.No);
                if(responce == MessageResult.Yes) {
                    var deletedConsumer = this._locationManager.ConsumerOperations.Delete(this.SelectedConsumer);
                    if(deletedConsumer != null) {
                        this.DispatcherService.BeginInvoke(new Action(() => {
                            this.MessageBoxService.ShowMessage("Consumer: " + deletedConsumer.Name + " Deleted", "Deleted", MessageButton.OK, MessageIcon.Exclamation);
                        }));
                        this._locationManager.Commit();
                        this.PopulateAsync();
                    } else {
                        this.DispatcherService.BeginInvoke(new Action(() => {
                            this.MessageBoxService.ShowMessage(" Error Deleting Consumer: " + this.SelectedConsumer.Name
                                + Environment.NewLine
                                + "Please Contact Administrator", "Error", MessageButton.OK, MessageIcon.Error);
                        }));
                        this._locationManager.UndoChanges();
                        this.PopulateAsync();
                    }
                } else {
                    this.DispatcherService.BeginInvoke(new Action(() => {
                        this.MessageBoxService.ShowMessage("No Changes Made, Reloading! ", "Action Canceled", MessageButton.OK, MessageIcon.Error);
                    }));
                    this._locationManager.UndoChanges();
                    this.PopulateAsync();
                }
            }
        }

        private void NewConsumerHandler() {
            NavigationParameters param = new NavigationParameters();
            Consumer newConsumer = new Consumer();
            param.Add("Consumer", newConsumer);
            param.Add("IsNew", true);
            this._regionManager.RequestNavigate(Regions.LocationDetailsRegion, AppViews.ConsumerDetailsView, param);
        }

        private void UpdateConsumerHandler(Consumer consumer) {
            if(consumer != null) {
                var toUpdate = this._locationManager.ConsumerOperations.Update(consumer);
                if(toUpdate != null) {
                    this.DispatcherService.BeginInvoke(new Action(() => {
                        this.MessageBoxService.ShowMessage("Consumer: " + toUpdate.Name + " saved!", "Saved", MessageButton.OK, MessageIcon.Information);
                    }));
                    this._locationManager.Commit();
                    this.PopulateAsync();
                } else {
                    this.DispatcherService.BeginInvoke(new Action(() => {
                        this.MessageBoxService.ShowMessage(" Error Updating Consumer: " + consumer.Name
                            + Environment.NewLine
                            + "Please Contact Administrator", "Error", MessageButton.OK, MessageIcon.Error);
                    }));
                    this._locationManager.UndoChanges();
                    this.PopulateAsync();
                }
            }
        }

        private void SaveConsumerHandler(Consumer consumer) {
            if(consumer != null) {
                var toAdd = this._locationManager.ConsumerOperations.Add(consumer);
                if(toAdd != null) {
                    this.DispatcherService.BeginInvoke(new Action(() => {
                        this.MessageBoxService.ShowMessage("Consumer: " + toAdd.Name + " saved!",
                            "Saved", MessageButton.OK, MessageIcon.Information);
                    }));
                    this._locationManager.Commit();
                    this.PopulateAsync();
                } else {
                    this.DispatcherService.BeginInvoke(new Action(() => {
                        this.MessageBoxService.ShowMessage(" Error Adding Consumer: " + consumer.Name
                            + Environment.NewLine
                            + "Please Contact Administrator", "Error", MessageButton.OK, MessageIcon.Error);
                    }));
                    this._locationManager.UndoChanges();
                    this.PopulateAsync();
                }
            }
        }

        private void DiscardConsumerChangesHandler() {
            this.DispatcherService.BeginInvoke(new Action(() => {
                this.MessageBoxService.ShowMessage("Changes Discared,Reloading", "Discarded", MessageButton.OK, MessageIcon.Information);
            }));
            this._locationManager.UndoChanges();
            this._regionManager.Regions[Regions.LocationDetailsRegion].RemoveAll();
            this.PopulateAsync();
        }

        private void Populate() {
            this.Consumers = this._locationManager.ConsumerProvider.GetEntityList().ToList();
        }

        private async void PopulateAsync() {
            this.Consumers = (await this._locationManager.ConsumerProvider.GetEntityListAsync()).ToList();
        }

        private void LoadedHandler() {
            this._locationManager.ConsumerProvider.LoadData();
        }

        private async void LoadedAsyncHandler() {
            await this._locationManager.ConsumerProvider.LoadDataAsync();
        }
    }
}