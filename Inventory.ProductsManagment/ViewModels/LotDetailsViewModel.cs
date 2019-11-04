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
using Inventory.Common.DataLayer.Providers;
using System.Windows;

namespace Inventory.ProductsManagment.ViewModels {
    public class LotDetailsViewModel : InventoryViewModelNavigationBase {

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private ProductDataManager _dataManager;
        public IMessageBoxService MessageBoxService { get=>ServiceContainer.GetService<IMessageBoxService>("LotDetailsNotifications"); } 
        public IDispatcherService Dispatcher { get => ServiceContainer.GetService<IDispatcherService>("LotDispatcher"); }

        private Lot _selectedLot = new Lot();
        private bool _isEdit = false;
        private Visibility _visibility = Visibility.Collapsed;

        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand DiscardCommand { get; private set; }


        public LotDetailsViewModel(ProductDataManager dataManager,IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this._dataManager=dataManager;
            this.SaveCommand = new AsyncCommand(this.SaveAsyncHandler);
            this.DiscardCommand = new AsyncCommand(this.DiscardAsyncHandler);
        }

        public Lot SelectedLot {
            get => this._selectedLot;
            set => SetProperty(ref this._selectedLot, value, "SelectedLot");
        }

        public bool IsEdit {
            get => this._isEdit;
            set => SetProperty(ref this._isEdit, value);
        }

        public Visibility Visibility {
            get => this._visibility;
            set => SetProperty(ref this._visibility, value);
        }

        public override bool KeepAlive {
            get => false;
        }

        private async Task SaveAsyncHandler() {
            await Task.Run(() => {
                if(this.SelectedLot != null) {
                    var updated = this._dataManager.LotOperations.Update(this.SelectedLot);
                    if(updated != null) {
                        this.Visibility = Visibility.Collapsed;
                        this.Dispatcher.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Lot Saved", "Success", MessageButton.OK, MessageIcon.Information);
                        });
                        this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>().Publish();
                    } else {
                        var responce = this.MessageBoxService.ShowMessage("Lot Save Failed"
                                + Environment.NewLine
                                + "Would You Like To Reload Original Values?"
                                + Environment.NewLine
                                + Environment.NewLine
                                + "Proess Yes to Reload Original Values"
                                + Environment.NewLine
                                + "Press No to Try Saving Again", "Error", MessageButton.YesNo, MessageIcon.Error, MessageResult.No);
                        if(responce == MessageResult.Yes) {
                            var original = this._dataManager.LotProvider.GetEntity(e => e.LotNumber == this.SelectedLot.LotNumber && e.SupplierPoNumber == this.SelectedLot.SupplierPoNumber);
                            if(original != null) {
                                this.SelectedLot = original;
                                this.Visibility = Visibility.Collapsed;
                                this.Dispatcher.BeginInvoke(() => {
                                    this.MessageBoxService.ShowMessage("Values Reloaded", "", MessageButton.OK, MessageIcon.Information);
                                });
                            } else {
                                this.Visibility = Visibility.Collapsed;
                                this.Dispatcher.BeginInvoke(() => {
                                    this.MessageBoxService.ShowMessage("Error Reloading, Please Select Lot Again to View Detials ", "", MessageButton.OK, MessageIcon.Error);
                                });
                            }
                            this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>().Publish();
                        } else {
                            this.Dispatcher.BeginInvoke(() => {
                                this.MessageBoxService.ShowMessage("Please Check Inputs and Try Saving Again", "", MessageButton.OK, MessageIcon.Information);
                            });
                        }
                    }
                }
            });
        }

        private async Task DiscardAsyncHandler() {
            await Task.Run(() => {
                if(this.SelectedLot != null) {
                    var original = this._dataManager.LotProvider.GetEntity(e => e.LotNumber == this.SelectedLot.LotNumber && e.SupplierPoNumber == this.SelectedLot.SupplierPoNumber);
                    if(original != null) {
                        this.Visibility = Visibility.Collapsed;
                        this.Dispatcher.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Changes Discarded", "", MessageButton.OK, MessageIcon.Information);
                        });
                    } else {
                        this.Visibility = Visibility.Collapsed;
                        this.Dispatcher.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Error Reloading, Please Select Lot Again to View Detials ", "", MessageButton.OK, MessageIcon.Error);
                        });
                    }
                    this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>().Publish();
                }
            });
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            var lot = navigationContext.Parameters["Lot"] as Lot;
            if(lot != null) {
                return this.SelectedLot != null && this.SelectedLot.LotNumber == lot.LotNumber && this.SelectedLot.SupplierPoNumber == lot.SupplierPoNumber;
            } else {
                return true;
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var lot = navigationContext.Parameters["Lot"] as Lot;
            var isEdit = Convert.ToBoolean(navigationContext.Parameters["IsEdit"]);
            if(lot != null) {
                this.SelectedLot = lot;
                this.IsEdit = isEdit;
                if(this.IsEdit) {
                    this.Visibility = Visibility.Visible;
                } else {
                    this.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}