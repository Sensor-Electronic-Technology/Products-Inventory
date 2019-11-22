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
using System.Collections.ObjectModel;
using System.IO;
using System.Diagnostics;

namespace Inventory.ProductsManagment.ViewModels {
    public class LotDetailsViewModel : InventoryViewModelNavigationBase {

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private ProductDataManager _dataManager;
        private ObservableCollection<Attachment> _attachments = new ObservableCollection<Attachment>();
        private Attachment _selectedAttachment = new Attachment();
        private FileNameViewModel _fileNameViewModel = null;


        public IMessageBoxService MessageBoxService { get=>ServiceContainer.GetService<IMessageBoxService>("LotDetailsNotifications"); } 
        public IDispatcherService Dispatcher { get => ServiceContainer.GetService<IDispatcherService>("LotDispatcher"); }
        protected IDialogService FileNameDialog { get { return ServiceContainer.GetService<IDialogService>("LotFileNameDialog"); } }
        protected IOpenFileDialogService OpenFileDialogService { get { return ServiceContainer.GetService<IOpenFileDialogService>("LotOpenFileDialog"); } }
        protected ISaveFileDialogService SaveFileDialogService { get { return ServiceContainer.GetService<ISaveFileDialogService>("LotSaveFileDialog"); } }

        private Lot _selectedLot = new Lot();
        private bool _isEdit = false;
        private Visibility _visibility = Visibility.Collapsed;

        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand DiscardCommand { get; private set; }

        public PrismCommands.DelegateCommand NewAttachmentCommand { get; private set; }
        public PrismCommands.DelegateCommand DeleteAttachmentCommand { get; private set; }
        public PrismCommands.DelegateCommand DownloadFileCommand { get; private set; }
        public PrismCommands.DelegateCommand OpenFileCommand { get; private set; }


        public LotDetailsViewModel(ProductDataManager dataManager,IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this._dataManager=dataManager;
            this.SaveCommand = new AsyncCommand(this.SaveAsyncHandler);
            this.DiscardCommand = new AsyncCommand(this.DiscardAsyncHandler);
            this.NewAttachmentCommand = new PrismCommands.DelegateCommand(this.NewAttachmentHandler);
            this.DeleteAttachmentCommand = new PrismCommands.DelegateCommand(this.DeleteAttachmentHandler);
            this.OpenFileCommand = new PrismCommands.DelegateCommand(this.OpenFileHandler);
            this.DownloadFileCommand = new PrismCommands.DelegateCommand(this.DownloadFileHandler);
            this.Filter = Constants.FileFilters;
            this.FilterIndex = 12;
            this.Title = "Save File As";
            this.DefaultExt = "*.*";
            this.OverwritePrompt = true;
        }

        public string Filter { get; set; }
        public int FilterIndex { get; set; }
        public string Title { get; set; }
        public bool DialogResult { get; protected set; }
        public string ResultFileName { get; protected set; }
        public string FileBody { get; set; }
        public string DefaultExt { get; set; }
        public string DefaultFileName { get; set; }
        public bool OverwritePrompt { get; set; }

        public Lot SelectedLot {
            get => this._selectedLot;
            set => SetProperty(ref this._selectedLot, value, "SelectedLot");
        }

        public Attachment SelectedAttachment {
            get => this._selectedAttachment;
            set => SetProperty(ref this._selectedAttachment, value, "SelectedAttachment");
        }

        public ObservableCollection<Attachment> Attachments {
            get => this._attachments;
            set => SetProperty(ref this._attachments, value, "Attachments");
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

        private void NewAttachmentHandler() {
            this.OpenFileDialogService.Filter = Constants.FileFilters;
            this.OpenFileDialogService.FilterIndex = this.FilterIndex;
            this.OpenFileDialogService.Title = "Select File To Uplaod";
            var resp = this.OpenFileDialogService.ShowDialog();
            if (resp) {
                var file = this.OpenFileDialogService.File;
                string ext=Path.GetExtension(file.GetFullName());
                string tempFileName = file.Name.Substring(0, file.Name.IndexOf("."));

                if (File.Exists(file.GetFullName())) {
                    if (this.ShowAttachmentDialog(tempFileName)) {
                        if (this._fileNameViewModel != null) {
                            string dest = Path.Combine(Constants.DestinationDirectory, this._fileNameViewModel.FileName + ext);
                            if (!File.Exists(dest)) {
                                bool success = true;
                                try {
                                    File.Copy(file.GetFullName(), dest);
                                } catch {
                                    this.MessageBoxService.ShowMessage("Copy File Error");
                                    success = false;
                                }
                                if (success) {
                                    Attachment attachment = new Attachment(DateTime.Now, this._fileNameViewModel.FileName, "");
                                    attachment.Description = this._fileNameViewModel.Description;
                                    attachment.LotNumber = this.SelectedLot.LotNumber;
                                    attachment.SupplierPoNumber = this.SelectedLot.SupplierPoNumber;
                                    attachment.FileReference = dest;
                                    attachment.Extension = ext;

                                    var temp = this._dataManager.UploadLotAttachment(attachment);
                                    if (temp.Success) {
                                        this.MessageBoxService.ShowMessage(temp.Message, "Success", MessageButton.OK, MessageIcon.Information);
                                        this.SelectedLot.Attachments.Add(attachment);
                                        ObservableCollection<Attachment> attachTemp = new ObservableCollection<Attachment>();
                                        attachTemp.AddRange(this.SelectedLot.Attachments.ToArray());
                                        this.Attachments = attachTemp;
                                    } else {
                                        this.MessageBoxService.ShowMessage(temp.Message, "Failed", MessageButton.OK, MessageIcon.Error);
                                    }
                                }
                            } else {
                                this.MessageBoxService.ShowMessage("File Name already exist, Please try again", "Failed", MessageButton.OK, MessageIcon.Error);
                            }
                        }
                    }
                } else {
                    this.MessageBoxService.ShowMessage("Internal Error: File Not Found", "File Not Found", MessageButton.OK, MessageIcon.Error);
                }
                this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>();
            }
        }

        private void DeleteAttachmentHandler() {
            if (this.SelectedAttachment != null) {
                string message = "You are about to delete attachment:" + this.SelectedAttachment.Name +
                    Environment.NewLine + "Continue?";
                var result = this.MessageBoxService.ShowMessage(message, "Delete", MessageButton.YesNo, MessageIcon.Asterisk);
                if (result == MessageResult.Yes) {

                    if (File.Exists(this.SelectedAttachment.FileReference)) {
                        var success = true;
                        try {
                            File.Delete(this.SelectedAttachment.FileReference);
                        } catch {
                            this.MessageBoxService.ShowMessage("Failed to Delete Attachment", "Error", MessageButton.OK, MessageIcon.Error);
                            success = false;
                        }
                        if (success) {
                            var responce = this._dataManager.DeleteLotAttachment(this.SelectedAttachment);
                            if (responce.Success) {
                                this.SelectedLot.Attachments.Remove(this.SelectedAttachment);
                                ObservableCollection<Attachment> attachTemp = new ObservableCollection<Attachment>();
                                attachTemp.AddRange(this.SelectedLot.Attachments.ToArray());
                                this.Attachments = attachTemp;
                                this.MessageBoxService.ShowMessage(responce.Message, "Success", MessageButton.OK, MessageIcon.Information);
                            } else {
                                this.MessageBoxService.ShowMessage("", "Error", MessageButton.OK, MessageIcon.Error);
                            }
                        }
                    }
                    this._eventAggregator.GetEvent<LotRankReservationEditingDoneEvent>();
                }
            }
        }

        private void OpenFileHandler() {
            if (this.SelectedAttachment != null) {
                if (File.Exists(this.SelectedAttachment.FileReference)) {
                    Process.Start(this.SelectedAttachment.FileReference);
                }
            }
        }

        private void DownloadFileHandler() {
            if (this.SelectedAttachment != null) {
                if (File.Exists(this.SelectedAttachment.FileReference)) {
                    string file = this.SelectedAttachment.FileReference;
                    string ext = Path.GetExtension(file);
                    this.SaveFileDialogService.DefaultExt = ext;
                    this.SaveFileDialogService.DefaultFileName = Path.GetFileName(file);
                    this.SaveFileDialogService.Filter = this.Filter;
                    this.SaveFileDialogService.FilterIndex = this.FilterIndex;
                    this.DialogResult = SaveFileDialogService.ShowDialog();
                    if (this.DialogResult) {
                        File.Copy(file, this.SaveFileDialogService.File.GetFullName());
                    }
                } else {
                    this.MessageBoxService.ShowMessage("File doesn't exist??");
                }
            } else {
                this.MessageBoxService.ShowMessage("Selection is null??");
            }
        }

        private bool ShowAttachmentDialog(string currentFile) {
            if (this._fileNameViewModel == null) {
                this._fileNameViewModel = new FileNameViewModel(currentFile);
            }
            this._fileNameViewModel.FileName = currentFile;
            this._fileNameViewModel.Description = "";

            UICommand saveCommand = new UICommand() {
                Caption = "Save Attachment",
                IsCancel = false,
                IsDefault = true,
            };

            UICommand cancelCommand = new UICommand() {
                Id = MessageBoxResult.Cancel,
                Caption = "Cancel",
                IsCancel = true,
                IsDefault = false,
            };
            UICommand result = FileNameDialog.ShowDialog(
            dialogCommands: new List<UICommand>() { saveCommand, cancelCommand },
            title: "New Attachment Dialog",
            viewModel: this._fileNameViewModel);
            return result == saveCommand;
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
                ObservableCollection<Attachment> attachTemp = new ObservableCollection<Attachment>();
                attachTemp.AddRange(this.SelectedLot.Attachments.ToArray());
                this.Attachments = attachTemp;
                if (this.IsEdit) {
                    this.Visibility = Visibility.Visible;
                } else {
                    this.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}