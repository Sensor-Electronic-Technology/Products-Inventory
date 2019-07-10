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
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;

namespace Inventory.CategoriesManagment.ViewModels {
    public class ProductTypeTableViewModel : InventoryViewModelBase {

        private CategoryDataManager _dataManager;
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("ProductTypeTableNotifications"); } }
        public IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("ProductTypeTableDispatcher"); } }

        private List<ProductType> _productTypes = new List<ProductType>();
        private ProductType _selectedPackageType = new ProductType();

        public PrismCommands.DelegateCommand NewPackageTypeCommand { get; private set; }
        public PrismCommands.DelegateCommand CategorySelectedCommand { get; private set; }
        public PrismCommands.DelegateCommand DeletePackageTypeCommand { get; private set; }

        public ProductTypeTableViewModel(CategoryDataManager dataManager, IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._dataManager = dataManager;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            this.NewPackageTypeCommand = new PrismCommands.DelegateCommand(this.NewPackageTypeHandler);
            this.CategorySelectedCommand = new PrismCommands.DelegateCommand(this.CategorySelectedHandler);
            this.DeletePackageTypeCommand = new PrismCommands.DelegateCommand(this.DeleteCategoryHandler);

            this._eventAggregator.GetEvent<SaveNewCategoryEvent>().Subscribe(this.SaveNewCategoryHandler);
            this._eventAggregator.GetEvent<SaveCategoryEvent>().Subscribe(this.SaveCategoryHandler);
            this._eventAggregator.GetEvent<DiscardCategoryEvent>().Subscribe(this.DiscardCategoryHandler);

            this.PopulateAsync();
        }

        public List<ProductType> PackageTypes {
            get => this._productTypes;
            set => SetProperty(ref this._productTypes, value, "PackageTypes");
        }

        public ProductType SelectedCategory {
            get => this._selectedPackageType;
            set => SetProperty(ref this._selectedPackageType, value, "SelectedPackageType");
        } 

        public override bool KeepAlive {
            get => false;
        }

        public void NewPackageTypeHandler() {
            ProductType productType = new ProductType();
            NavigationParameters param = new NavigationParameters();
            param.Add("Category", productType);
            param.Add("IsNew", true);
            this._regionManager.RequestNavigate(Regions.CategoryDetailsRegion, AppViews.CategoryDetailsView, param);
        }

        public void CategorySelectedHandler() {
            if(this.SelectedCategory != null) { 
                    NavigationParameters param = new NavigationParameters();
                    param.Add("Category", this.SelectedCategory);
                    param.Add("IsNew", false);
                    this._regionManager.RequestNavigate(Regions.CategoryDetailsRegion, AppViews.CategoryDetailsView, param);
            }
        }

        private void SaveCategoryHandler(Category category) {
            var entity=this._dataManager.ProductTypeOperations.Update((ProductType)category);
            if(entity == null) {
                this.DispatcherService.BeginInvoke(new Action(() => {
                    this.MessageBoxService.ShowMessage("Error Saving Changes, All Changes Discarded","Error",MessageButton.OK,MessageIcon.Error);
                }));
            } else {
                this.DispatcherService.BeginInvoke(new Action(() => {
                    this.MessageBoxService.ShowMessage("Changes Saved", "Success", MessageButton.OK, MessageIcon.Asterisk);
                }));
                this._dataManager.Commit();
                this._eventAggregator.GetEvent<ReloadCategoriesEvent>().Publish();
                this.PopulateAsync();
            }
        }

        private void SaveNewCategoryHandler(Category category) {
            var entity=this._dataManager.ProductTypeOperations.Add((ProductType)category);
            if(entity!=null) {
                this.DispatcherService.BeginInvoke(new Action(() => {
                    this.MessageBoxService.ShowMessage("New Package Type Saved", "Success", MessageButton.OK, MessageIcon.Asterisk);
                }));
                this._dataManager.Commit();
                this._eventAggregator.GetEvent<ReloadCategoriesEvent>().Publish();
                this.PopulateAsync();
            } else {
                this.DispatcherService.BeginInvoke(new Action(() => {
                    this.MessageBoxService.ShowMessage("Error Saving New Category,No Changes Made", "Error", MessageButton.OK, MessageIcon.Error);
                }));
            }
        }

        private void DiscardCategoryHandler() {
            this._dataManager.UndoChanges();
            this.DispatcherService.BeginInvoke(new Action(() => {
                this.MessageBoxService.ShowMessage("Changes Discarded", "Discarded", MessageButton.OK, MessageIcon.Exclamation);
            }));
            this.PopulateAsync();
        }

        private void DeleteCategoryHandler() {
            if(this.SelectedCategory != null) {
                var responce=this.MessageBoxService.ShowMessage("You are about to delete Package Type:  "
                    + Environment.NewLine
                    + this.SelectedCategory.Name
                    + "Continue?"
                    + "WARNING:  This cannot be undone! "
                    , "Warning"
                    , MessageButton.YesNo
                    , MessageIcon.Warning
                    , MessageResult.No);
                if(responce == MessageResult.Yes) {
                    var entity=this._dataManager.ProductTypeOperations.Delete(this.SelectedCategory);
                    if(entity != null) {
                        this.DispatcherService.BeginInvoke(new Action(() => {
                            this.MessageBoxService.ShowMessage("Package Type: "+entity.Name+" Deleted", "Deleted", MessageButton.OK, MessageIcon.Exclamation);
                        }));
                        this._dataManager.Commit();
                        this.PopulateAsync();
                    } else {
                        this.DispatcherService.BeginInvoke(new Action(() => {
                            this.MessageBoxService.ShowMessage(" Error Deleting Package Type: " + entity.Name
                                +Environment.NewLine
                                +"Please Contact Administrator", "Error", MessageButton.OK, MessageIcon.Error);
                        }));
                        this._dataManager.UndoChanges();
                        this.PopulateAsync();
                    }
                }
            }
        }

        private async void PopulateAsync() {
            this.PackageTypes = (await this._dataManager.ProductTypeProvider.GetEntityListAsync()).ToList();
        }
    }
}