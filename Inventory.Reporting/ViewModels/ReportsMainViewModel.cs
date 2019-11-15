using Prism.Regions;
using DevExpress.Mvvm;
using Inventory.Common.ApplicationLayer;
using DevExpress.Xpf.Core;
using Inventory.Common.BuisnessLayer;
using PrismCommands = Prism.Commands;
using System.Threading.Tasks;
using System;
using System.Linq;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.Common.ApplicationLayer.Services;
using Inventory.Common.EntityLayer.Model;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Diagnostics;
using System.Data;

namespace Inventory.Reporting.ViewModels {
    public class ReportsMainViewModel : InventoryViewModelBase {
        private IRegionManager _regionManager;
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("ReportsMainNotice"); } }
        public IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("MainDispatcherService"); }

        public AsyncCommand<string> LoadReportsViewCommand { get; set; }

        public ReportsMainViewModel(IRegionManager regionManager) {
            this._regionManager = regionManager;
            this.LoadReportsViewCommand = new AsyncCommand<string>(this.LoadViewHandler);
        }

        public override bool KeepAlive {
            get => true;
        }

        private async Task LoadViewHandler(string navigationPath) {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    if (!string.IsNullOrEmpty(navigationPath)) {
                        this._regionManager.RequestNavigate(Regions.ReportsMainRegion, navigationPath);
                    }
                });
            });
        }
    }
}