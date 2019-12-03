using System;
using System.Text;
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
using Inventory.Common.DataLayer.Providers;
using System.Windows.Input;
using System.Windows;
using Inventory.Common.ApplicationLayer.Services;
using System.Threading;

namespace Inventory.PartsManagment.ViewModels {
    public class PartInstanceDetailsViewModel : InventoryViewModelBase {

        public override bool KeepAlive {
            get => false;
        }

    }
}