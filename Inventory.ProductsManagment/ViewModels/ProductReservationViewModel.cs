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
    public class ProductReservationViewModel : InventoryViewModelNavigationBase {
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private ProductDataManager _dataManager;
        public IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("ReservationNotifications"); }
        public IDispatcherService Dispatcher { get => ServiceContainer.GetService<IDispatcherService>("ReservationDispatcher"); }

        private ProductReservation _selectedReservation = new ProductReservation();
        private string _lotPo;
        bool _isEdit = false;
        bool _isNew = false;
        private Visibility _visibility = Visibility.Collapsed;

        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand DiscardCommand { get; private set; }

        public ProductReservationViewModel(ProductDataManager dataManager, IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this._dataManager = dataManager;
            this.SaveCommand = new AsyncCommand(this.SaveAsyncHandler);
            this.DiscardCommand = new AsyncCommand(this.DiscardAsyncHandler);
        }

        public ProductReservation SelectedReservation {
            get => this._selectedReservation;
            set => SetProperty(ref this._selectedReservation, value);
        }

        public string LotPo {
            get => this._lotPo;
            set => SetProperty(ref this._lotPo, value);
        }

        public bool IsEdit {
            get => this._isEdit;
            set => SetProperty(ref this._isEdit, value);
        }

        public bool IsNew {
            get => this._isNew;
            set => SetProperty(ref this._isNew, value);
        }

        public Visibility Visibility {
            get => this._visibility;
            set => SetProperty(ref this._visibility, value);
        }

        public override bool KeepAlive {
            get => false;
        }

        private async Task DiscardAsyncHandler() {
            await Task.Run(() => {
                if(this.SelectedReservation != null) {
                    var original = this._dataManager.ReservationProvider.GetEntity(e => e.Id == this.SelectedReservation.Id);
                    if(original != null) {
                        this.Visibility = Visibility.Collapsed;
                        this.Dispatcher.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Changes Discarded", "", MessageButton.OK, MessageIcon.Information);
                        });
                    } else {
                        this.Visibility = Visibility.Collapsed;
                        this.Dispatcher.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Error Reloading, Please Select Reservation Again to View Detials ", "", MessageButton.OK, MessageIcon.Error);
                        });
                    }
                    this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>().Publish();
                }
            });
        }

        private async Task SaveAsyncHandler() {
            await Task.Run(() => {
                if(!this.IsNew) {
                    var added = this._dataManager.ReservationOperations.Add(this.SelectedReservation);
                    if(added != null) {
                        this.Visibility = Visibility.Collapsed;
                        this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>().Publish();
                    } else {
                        var responce = this.MessageBoxService.ShowMessage("Reservation Save Failed"
                            + Environment.NewLine
                            + "Would You Like To Reload Original Values?"
                            + Environment.NewLine
                            + Environment.NewLine
                            + "Proess Yes to Reload Original Values"
                            + Environment.NewLine
                            + "Press No to Try Saving Again", "Error", MessageButton.YesNo, MessageIcon.Error, MessageResult.No);
                        if(responce == MessageResult.Yes) {
                            var original = this._dataManager.ReservationProvider.GetEntity(e => e.Id == this.SelectedReservation.Id);
                            if(original != null) {
                                this.SelectedReservation = original;
                                this.Visibility = Visibility.Collapsed;
                                this.Dispatcher.BeginInvoke(() => {
                                    this.MessageBoxService.ShowMessage("Values Reloaded", "", MessageButton.OK, MessageIcon.Information);
                                });
                            } else {
                                this.Visibility = Visibility.Collapsed;
                                this.Dispatcher.BeginInvoke(() => {
                                    this.MessageBoxService.ShowMessage("Error Reloading, Please Select Reservation Again to View Detials ", "", MessageButton.OK, MessageIcon.Error);
                                });
                            }
                            this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>().Publish();
                        } else {
                            this.Dispatcher.BeginInvoke(() => {
                                this.MessageBoxService.ShowMessage("Please Check Inputs and Try Saving Again", "", MessageButton.OK, MessageIcon.Information);
                            });
                        }
                    }
                } else if(!this.IsEdit) {
                    var updated = this._dataManager.ReservationOperations.Update(this.SelectedReservation);
                    if(updated != null) {
                        this.Visibility = Visibility.Collapsed;
                        this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>().Publish();
                        this.Dispatcher.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Changes Saved", "", MessageButton.OK, MessageIcon.Information);
                        });
                    } else {
                        var responce = this.MessageBoxService.ShowMessage("Reservation Save Failed"
                            + Environment.NewLine
                            + "Would You Like To Reload Original Values?"
                            + Environment.NewLine
                            + Environment.NewLine
                            + "Proess Yes to Reload Original Values"
                            + Environment.NewLine
                            + "Press No to Try Saving Again", "Error", MessageButton.YesNo, MessageIcon.Error, MessageResult.No);
                        if(responce == MessageResult.Yes) {
                            var original = this._dataManager.ReservationProvider.GetEntity(e => e.Id == this.SelectedReservation.Id);
                            if(original != null) {
                                this.SelectedReservation = original;
                                this.Visibility = Visibility.Collapsed;
                                this.Dispatcher.BeginInvoke(() => {
                                    this.MessageBoxService.ShowMessage("Values Reloaded", "", MessageButton.OK, MessageIcon.Information);
                                });
                            } else {
                                this.Visibility = Visibility.Collapsed;
                                this.Dispatcher.BeginInvoke(() => {
                                    this.MessageBoxService.ShowMessage("Error Reloading, Please Select Reservation Again to View Detials ", "", MessageButton.OK, MessageIcon.Error);
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

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            var reservation = navigationContext.Parameters["Reservation"] as ProductReservation;
            if(reservation != null) {
                return this.SelectedReservation != null && this.SelectedReservation.Id == reservation.Id;
            } else {
                return true;
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var reservation = navigationContext.Parameters["Reservation"] as ProductReservation;
            var isEdit = Convert.ToBoolean(navigationContext.Parameters["IsEdit"]);
            var isNew = Convert.ToBoolean(navigationContext.Parameters["IsNew"]);
            if(reservation != null) {
                this.SelectedReservation = reservation;
                this.IsEdit = !isEdit;
                this.IsNew = !isNew;
                this.Visibility = (!this.IsEdit || !this.IsNew) ? Visibility.Visible : Visibility.Collapsed;
                this.LotPo = "(" + this.SelectedReservation.LotNumber + "," + this.SelectedReservation.PoNumber + ")";
            }
        }
    }
}