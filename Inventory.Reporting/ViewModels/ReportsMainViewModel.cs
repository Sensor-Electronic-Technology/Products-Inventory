using Prism.Regions;
using DevExpress.Mvvm;
using Inventory.Common.ApplicationLayer;
using DevExpress.Xpf.Core;
using Inventory.Common.BuisnessLayer;
using PrismCommands = Prism.Commands;

namespace Inventory.Reporting.ViewModels {
    public class ReportsMainViewModel : InventoryViewModelBase {

        private IRegionManager _regionManager;
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("ReportsMainNotice"); } }

        public PrismCommands.DelegateCommand<string> LoadReportsViewCommand { get; set; }

        public ReportsMainViewModel(IRegionManager regionManager) {
            this._regionManager = regionManager;
            this.LoadReportsViewCommand = new PrismCommands.DelegateCommand<string>(this.LoadViewHandler);
        }

        public override bool KeepAlive {
            get => true;
        }

        private void LoadViewHandler(string navigationPath) {
            if (!string.IsNullOrEmpty(navigationPath)) {
                this._regionManager.RequestNavigate(Regions.ReportsMainRegion, navigationPath);
            }
        }
    }
}