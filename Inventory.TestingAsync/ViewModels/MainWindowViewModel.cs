using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.Common.ApplicationLayer;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using System;
using Inventory.Common.ApplicationLayer.UI_Services;
using System.Diagnostics;
using System.Threading;

namespace Inventory.TestingAsync.ViewModels
{
    public class MainWindowViewModel : InventoryViewModelBase {
        private string _title = "Async Testing";
        private List<Product> _products = new List<Product>();
        private InventoryContext _context;
        private string _asyncText = "Text Here";
        private string _syncText = "Text Here";
        private Stopwatch stopwatch = new Stopwatch();
        private object SyncRoot = new object();

        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("Notify"); } }
        public IDispatcherService Dispatcher { get { return ServiceContainer.GetService<IDispatcherService>("Dispatcher"); } }
        public IControlUpdateService GridUpdateService { get { return ServiceContainer.GetService<IGridUpdateService>(); } }
        public ISplashScreenService SplashScreenService { get { return ServiceContainer.GetService<ISplashScreenService>(); } }

        public AsyncCommand AsyncCommandTest1 { get; private set; }
        public DelegateCommand SynchronousCommand { get; private set; }

        public MainWindowViewModel() {

            this.AsyncCommandTest1 = new AsyncCommand(this.LoadAsyncVar2);
            this.SynchronousCommand = new DelegateCommand(this.Load);
        }

        public List<Product> Products {
            get => this._products;
            set => SetProperty(ref this._products, value, "Products");
        }

        public string AsyncText {
            get => this._asyncText;
            set => SetProperty(ref this._asyncText, value);
        }

        public string SyncText {
            get => this._syncText;
            set => SetProperty(ref this._syncText, value);
        }

        public string Title {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public override bool KeepAlive {
            get => true;
        }

        private async Task LoadHandler() {
             await Task.Run(() => this.Load());
        }

        private async void LoadAsync() {
            this.stopwatch.Reset();
            this.stopwatch.Start();

            await Task.Run(async () => {
                await this._context.InventoryItems.OfType<Product>()
                 .Include(e => e.Instances)
                 .Include(e => e.Lots)
                 .Include(e => e.ProductType)
                 .Include(e => e.Warehouse)
                 .LoadAsync();
                this.Products = await this._context.InventoryItems.OfType<Product>()
                 .Include(e => e.Instances)
                 .Include(e => e.Lots)
                 .Include(e => e.ProductType)
                 .Include(e => e.Warehouse)
                 .ToListAsync();
                await Task.Delay(2000);
            });
            await Task.Run(() => {
                stopwatch.Stop();
                this.AsyncText = this.stopwatch.ElapsedMilliseconds.ToString();
            });
        }

        private async Task LoadAsyncVar2() {
            
            await Task.Run(() => {
                this._context = new InventoryContext();
                this.stopwatch.Reset();
                this.stopwatch.Start();
            });

            await this._context.InventoryItems.OfType<Product>()
             .Include(e => e.Instances)
             .Include(e => e.Lots)
             .Include(e => e.ProductType)
             .Include(e => e.Warehouse)
             .LoadAsync();
            
            await Task.Run(() => {
                if(this.GridUpdateService != null) {
                    lock(this.SyncRoot) {
                        this.GridUpdateService.BeginUpdate();
                        this.Products = this._context.InventoryItems.OfType<Product>()
                             .Include(e => e.Instances)
                             .Include(e => e.Lots)
                             .Include(e => e.ProductType)
                             .Include(e => e.Warehouse)
                             .ToList();
                        this.GridUpdateService.EndUpdate();
                    }
                }
            });

            //await Task.Delay(2000);
            await Task.Run(() => {
                stopwatch.Stop();
                this.AsyncText = this.stopwatch.ElapsedMilliseconds.ToString();
            });
        }

        private void Load() {
            this._context = new InventoryContext();
            this.stopwatch.Reset();
            this.stopwatch.Start();
            this._context.InventoryItems.OfType<Product>()
                .Include(e => e.Instances)
                .Include(e => e.Lots)
                .Include(e => e.ProductType)
                .Include(e => e.Warehouse).Load();
            this.Products = this._context.InventoryItems.OfType<Product>().Include(e => e.Instances).Include(e => e.Lots).Include(e => e.ProductType).Include(e => e.Warehouse).ToList();
            stopwatch.Stop();
            this.SyncText = this.stopwatch.ElapsedMilliseconds.ToString();
        }
    }
}
