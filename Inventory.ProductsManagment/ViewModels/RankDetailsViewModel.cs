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
using System.Windows.Input;

namespace Inventory.ProductsManagment.ViewModels {
    public class RankDetailsViewModel : InventoryViewModelNavigationBase {

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private ProductDataManager _dataManager;
        public IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("RankDetailsNotifications"); }
        public IDispatcherService Dispatcher { get => ServiceContainer.GetService<IDispatcherService>("RankDispatcher"); }

        private ProductInstance _selectedRank = new ProductInstance();
        private ProductReservation _selectedReservation = new ProductReservation();
        private bool _isEdit = false;
        private bool _canEditQuantity = false;
        private Visibility _visibility=Visibility.Collapsed;

        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand DiscardCommand { get; private set; }
        public ICommand DeleteReservationCommand { get; private set; }

        public RankDetailsViewModel(ProductDataManager dataManager, IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this._dataManager = dataManager;
            this.SaveCommand = new AsyncCommand(this.SaveAsyncHandler);
            this.DiscardCommand = new AsyncCommand(this.DiscardAsyncHandler);
            this.DeleteReservationCommand = new PrismCommands.DelegateCommand(this.DeleteReservationHandler);
        }

        public ProductInstance SelectedRank {
            get => this._selectedRank;
            set => SetProperty(ref this._selectedRank, value, "SelectedRank");
        }

        public ProductReservation SelectedReservation {
            get => this._selectedReservation;
            set => SetProperty(ref this._selectedReservation, value);
        }

        public bool IsEdit {
            get => this._isEdit;
            set => SetProperty(ref this._isEdit, value);
        }

        public bool CanEditQuantity {
            get => this._canEditQuantity;
            set => SetProperty(ref this._canEditQuantity, value);
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
                if(this.SelectedRank != null) {
                    var updated = this._dataManager.RankOperations.Update(this.SelectedRank);
                    if(updated != null) {
                        this.Visibility = Visibility.Collapsed;
                        this.Dispatcher.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Rank Saved", "Success", MessageButton.OK, MessageIcon.Information);
                        });
                        this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>().Publish();
                    } else {
                        var responce = this.MessageBoxService.ShowMessage("Rank Save Failed"
                                + Environment.NewLine
                                + "Would You Like To Reload Original Values?"
                                + Environment.NewLine
                                + Environment.NewLine
                                + "Proess Yes to Reload Original Values"
                                + Environment.NewLine
                                + "Press No to Try Saving Again", "Error", MessageButton.YesNo, MessageIcon.Error, MessageResult.No);
                        if(responce == MessageResult.Yes) {
                            var original = this._dataManager.RankProvider.GetEntity(e => e.Id == this.SelectedRank.Id);
                            if(original != null) {
                                this.SelectedRank = original;
                                this.Visibility = Visibility.Collapsed;
                                this.Dispatcher.BeginInvoke(() => {
                                    this.MessageBoxService.ShowMessage("Values Reloaded", "", MessageButton.OK, MessageIcon.Information);
                                });
                            } else {
                                this.Visibility = Visibility.Collapsed;
                                this.Dispatcher.BeginInvoke(() => {
                                    this.MessageBoxService.ShowMessage("Error Reloading, Please Select Rank Again to View Detials ", "", MessageButton.OK, MessageIcon.Error);
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
                if(this.SelectedRank != null) {
                    var original = this._dataManager.RankProvider.GetEntity(e => e.Id == this.SelectedRank.Id);
                    if(original != null) {
                        this.Visibility = Visibility.Collapsed;
                        this.Dispatcher.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Changes Discarded", "", MessageButton.OK, MessageIcon.Information);
                        });
                    } else {
                        this.Visibility = Visibility.Collapsed;
                        this.Dispatcher.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Error Reloading, Please Select Rank Again to View Detials ", "", MessageButton.OK, MessageIcon.Error);
                        });
                    }
                    this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>().Publish();
                }
            });
        }

        private void DeleteReservationHandler() {
            if(this.SelectedReservation != null) {
                var responce = this.MessageBoxService.ShowMessage("You are about to delete Product Reservation"
                    + Environment.NewLine
                    + "This cannot be undone, Continue?"
                    , "Warning", MessageButton.YesNo, MessageIcon.Warning, MessageResult.No);
                if(responce == MessageResult.Yes) {
                    var deleted = this._dataManager.ReservationOperations.Delete(this.SelectedReservation);
                    if(deleted != null) {
                        this.Dispatcher.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Reservation Succesfully Deleted"
                                + Environment.NewLine + "Reloading Data"
                                , "Success", MessageButton.OK, MessageIcon.Information);
                        });
                        this.Reload();
                    } else {
                        this.Dispatcher.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Delete Failed, No Changes Made" 
                                + Environment.NewLine + "Reloading Data"
                                , "Error", MessageButton.OK, MessageIcon.Error);
                        });
                        this.Reload();
                    }
                } else {
                    this.Dispatcher.BeginInvoke(() => {
                        this.MessageBoxService.ShowMessage("Delete Canceled, No Changes Made."
                            +Environment.NewLine+"Reloading Data"
                            , "Error", MessageButton.OK, MessageIcon.Error);
                    });
                    this.Reload();
                }
            }
        }

        public void Reload() {
            var original = this._dataManager.RankProvider.GetEntity(e => e.Id == this.SelectedRank.Id);
            if(original != null) {
                this.SelectedRank = original;
                this.Dispatcher.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage("Values Reloaded", "", MessageButton.OK, MessageIcon.Information);
                });
            } else {
                this.Visibility = Visibility.Collapsed;
                this.Dispatcher.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage("Error Reloading, Please Select Rank Again to View Detials ", "", MessageButton.OK, MessageIcon.Error);
                });
            }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            var rank = navigationContext.Parameters["Rank"] as ProductInstance;
            if(rank != null) {
                return this.SelectedRank != null && this.SelectedRank.Name == rank.Name;
            } else {
                return true;
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var rank = navigationContext.Parameters["Rank"] as ProductInstance;
            var isEdit = Convert.ToBoolean(navigationContext.Parameters["IsEdit"]);
            var canEditQuantity = Convert.ToBoolean(navigationContext.Parameters["CanEditQuantity"]);
            if(rank != null) {
                this.SelectedRank = rank;
                this.IsEdit = !isEdit;
                this.CanEditQuantity = !canEditQuantity;
                if(!this.IsEdit) {
                    this.Visibility = Visibility.Visible;
                } else {
                    this.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}