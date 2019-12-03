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
using Inventory.Common.DataLayer.Providers;
using System.Windows.Input;
using System.Windows;
using Inventory.Common.ApplicationLayer.Services;
using System.Threading;
using Inventory.Common.EntityLayer.Model;
using System.Collections.ObjectModel;
using System.Data.Entity;

namespace Inventory.PartsManagment.ViewModels {
    public class PartsSelectorViewModel : InventoryViewModelBase {
        protected IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("PartSelectorNotifications"); } }
        protected IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("PartSelectorDispatcher"); } }

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private InventoryContext _context;

        private ObservableCollection<Part> _parts = new ObservableCollection<Part>();
        private Part _selectedPart = new Part();
        private bool _isLoading = false;
        private bool _isInitialized = false;
        private bool _editInProgress = false;

        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand RefreshDataCommand { get; private set; }
        public PrismCommands.DelegateCommand ViewPartDetailsCommand { get; private set; }
        public PrismCommands.DelegateCommand EditPartCommand { get; private set; }

        public PartsSelectorViewModel(InventoryContext context, IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this._context = context;
            this.InitializeCommand = new AsyncCommand(this.PopulateAsync);
        }


        public override bool KeepAlive {
            get => false;
        }

        public ObservableCollection<Part> Parts {
            get => this._parts;
            set => SetProperty(ref this._parts, value, "Parts");
        }

        public Part SelectedPart {
            get => this._selectedPart;
            set => SetProperty(ref this._selectedPart, value, "SelectedPart");
        }

        public bool IsLoading {
            get => this._isLoading;
            set => SetProperty(ref this._isLoading, value, "IsLoading");
        }

        private void ViewPartDetailsHandler() {
            if (this.SelectedPart != null) {
                this._regionManager.Regions[Regions.PartDetailsRegion].RemoveAll();
                this._regionManager.Regions[Regions.InstanceDetailsRegion].RemoveAll();
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Part", this.SelectedPart);
                parameters.Add("IsNew", false);
                parameters.Add("IsEdit", false);
                this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.PartInstanceDetailsView, parameters);
            }
        }

        private void EditPartDetailsHandler() {
            if (this.SelectedPart != null) {
                this._editInProgress = true;
                this._regionManager.Regions[Regions.PartDetailsRegion].RemoveAll();
                this._regionManager.Regions[Regions.InstanceDetailsRegion].RemoveAll();
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Part", this.SelectedPart);
                parameters.Add("IsNew", false);
                parameters.Add("IsEdit", true);
                this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.PartInstanceDetailsView, parameters);
            }
        }

        private void NewPartHandler() {
            if (this.SelectedPart != null) {
                this._editInProgress = true;
                this._regionManager.Regions[Regions.PartDetailsRegion].RemoveAll();
                this._regionManager.Regions[Regions.InstanceDetailsRegion].RemoveAll();
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Part", this.SelectedPart);
                parameters.Add("IsNew", true);
                parameters.Add("IsEdit", false);
                this._regionManager.RequestNavigate(Regions.ProductDetailsRegion, AppViews.PartInstanceDetailsView, parameters);
            }
        }

        private async Task PopulateAsync() {
            if (!this._isInitialized) {
                this.DispatcherService.BeginInvoke(() => this.IsLoading = true);
                await this._context.InventoryItems.OfType<Part>()
                    .Include(e => e.Instances.Select(i => i.Transactions))
                    .Include(e => e.Manufacturers)
                    .Include(e => e.Organization)
                    .Include(e => e.Usage)
                    .Include(e => e.Warehouse)
                    .LoadAsync();

                var tempParts=await this._context.InventoryItems.OfType<Part>()
                    .Include(e => e.Instances.Select(i => i.Transactions))
                    .Include(e => e.Manufacturers)
                    .Include(e => e.Organization)
                    .Include(e => e.Usage)
                    .Include(e => e.Warehouse)
                    .ToListAsync();

                var parts = new ObservableCollection<Part>();
                parts.AddRange(tempParts);
                this.Parts = parts;
                this._isInitialized = true;
                this.DispatcherService.BeginInvoke(() => this.IsLoading = false);
            }
        }
    }
}