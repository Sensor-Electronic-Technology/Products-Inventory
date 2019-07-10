using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inventory.Common.ApplicationLayer;
using Inventory.Common.ApplicationLayer.AsyncHelpers;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;

namespace Inventory.WPFThreadTesting.ViewModels {
    public class MainWindowViewModel : InventoryViewModelBase {

        private List<Product> _products=new List<Product>();
        private InventoryContext _context;
        private NotifyTask completed;
        private string _labelText="hello";


        public AsyncCommand ControlLoaded { get; private set; }
        public AsyncCommand AsyncCommandTest1 { get; private set; }
        public AsyncCommand AsyncCommandTest2 { get; private set; }

        public MainWindowViewModel(InventoryContext context) {
            this._context = context;
            this.AsyncCommandTest1 = new AsyncCommand(() =>Task.Run(() => this.Populate()));
            this.completed=NotifyTask.Create(this.AsyncCommandTest1.Execution.TaskCompleted);
            this._context.InventoryItems.OfType<Product>().Include(e => e.Instances).Include(e => e.Lots).Include(e => e.ProductType).Include(e => e.Warehouse).LoadAsync();
            this.LabelText = "Text Here";
        }

        private void Task_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            this.Populate();
        }

        public List<Product> Products {
            get => this._products;
            set => SetProperty(ref this._products, value,"Products");
        }

        public string LabelText {
            get => this._labelText;
            set => SetProperty(ref this._labelText, value, "LabelText");
        }

        public override bool KeepAlive {
            get => true;
        }

        private Task LoadHandler() {
            return this._context.InventoryItems.OfType<Product>().Include(e => e.Instances).Include(e => e.Lots).Include(e => e.ProductType).Include(e => e.Warehouse).LoadAsync();
        }

        private async void Populate() {
           this.Products=(await this._context.InventoryItems.OfType<Product>().Include(e => e.Instances).Include(e => e.Lots).Include(e => e.ProductType).Include(e => e.Warehouse).ToListAsync());
        }
    }
}