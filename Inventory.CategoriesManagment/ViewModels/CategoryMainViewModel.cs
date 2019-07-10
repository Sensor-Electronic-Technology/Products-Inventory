using DevExpress.Mvvm;
using Inventory.Common.ApplicationLayer;
using Prism.Regions;

namespace Inventory.CategoriesManagment.ViewModels {
    public class CategoryMainViewModel : InventoryViewModelBase {

        public IRegionManager _regionManager;
        public Prism.Commands.DelegateCommand<string> LoadCategoryViewCommand { get; private set; }

        public CategoryMainViewModel(IRegionManager regionManager) {
            this._regionManager = regionManager;
            this.LoadCategoryViewCommand = new Prism.Commands.DelegateCommand<string>(this.LoadCategoryViewHandler);
        }

        public override bool KeepAlive {
            get => true;
        }

        private void LoadCategoryViewHandler(string navigationPath) {
            if(!string.IsNullOrEmpty(navigationPath)) {
                this._regionManager.RequestNavigate(Regions.CategoryTableRegion, navigationPath);
            }
        }
    }
}