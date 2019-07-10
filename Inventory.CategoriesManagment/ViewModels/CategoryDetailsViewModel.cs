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
    public class CategoryDetailsViewModel : InventoryViewModelNavigationBase {

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;

        private Category _selectedCategory;
        private bool _isNew = false;

        public PrismCommands.DelegateCommand SaveCommand { get; private set; }
        public PrismCommands.DelegateCommand DiscardCommand { get; private set; }

        public CategoryDetailsViewModel(IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            this.SaveCommand = new PrismCommands.DelegateCommand(this.SaveCategoryHandler);
            this.DiscardCommand = new PrismCommands.DelegateCommand(this.DiscardCategoryHandler);
        }

        public Category SelectedCategory {
            get => this._selectedCategory;
            set => SetProperty(ref this._selectedCategory, value, "SelectedCategory");
        }

        public override bool KeepAlive {
            get => true;
        }

        private void SaveCategoryHandler() {
            if(this.SelectedCategory != null) {
                if(this._isNew) {
                    this._eventAggregator.GetEvent<SaveNewCategoryEvent>().Publish(this.SelectedCategory);
                } else {
                    this._eventAggregator.GetEvent<SaveCategoryEvent>().Publish(this.SelectedCategory);
                }
            }
        }

        private void DiscardCategoryHandler() {
            if(this.SelectedCategory != null)
                this._eventAggregator.GetEvent<DiscardCategoryEvent>().Publish();
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            var category = navigationContext.Parameters["Category"] as Category;
            this._isNew = Convert.ToBoolean(navigationContext.Parameters["IsNew"]);
            if(category != null) {
                return this.SelectedCategory != null && this.SelectedCategory.Name == category.Name;
            } else {
                return true;
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var category = navigationContext.Parameters["Category"] as Category;
            this._isNew = Convert.ToBoolean(navigationContext.Parameters["IsNew"]);
            if(category != null) {
                this.SelectedCategory = category;
            }
        }
    }
}