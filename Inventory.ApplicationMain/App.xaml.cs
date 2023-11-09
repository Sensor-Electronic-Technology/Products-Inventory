using System.Windows;
using DryIoc;
using Prism.Ioc;
using Prism.DryIoc;
using Prism.Modularity;
using Prism.Regions;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Prism;
using Inventory.ApplicationMain.Views;
using Inventory.ApplicationMain.ViewModels;
using Inventory.UsersManagment;
using Inventory.LocationManagement;
using Inventory.ProductsManagment;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.ProductSalesMain;
using Inventory.Common.DataLayer.Providers;
using Inventory.Common.BuisnessLayer;
using Inventory.CategoriesManagment;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using Inventory.Common.DataLayer.EntityDataManagers;
using DevExpress.Xpf.Grid;
using Inventory.Reporting;

namespace Inventory.ApplicationMain {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private IUserService userService=new UserService();

        protected override void OnStartup(StartupEventArgs e)
        {
            //Configure theme manager
            DXSplashScreen.Show<SETSplashScreen>();
            ApplicationThemeHelper.UpdateApplicationThemeName();
            ThemeManager.ApplicationThemeChanged += this.ThemeManager_ApplicationThemeChanged;
            GridControl.AllowInfiniteGridSize = true;

            /*DomainManager domainManager = new DomainManager();
            UserServiceProvider userServiceProvider = new UserServiceProvider(new InventoryContext(), domainManager);
            LogInService logInService = new LogInService(domainManager, userServiceProvider);
            var responce = logInService.LogInWithPassword("AElmendo", "Drizzle123!", false, InventorySoftwareType.PRODUCTS_SALES);
            this.userService = responce.Service;*/
           Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            if(this.ShowLogin()) {
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            } else {
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                this.Shutdown();
            }
            base.OnStartup(e);
        }

        private bool ShowLogin()
        {
            //Startup Login
            LoginWindow loginWindow = new LoginWindow();
            DomainManager domainManager = new DomainManager();
            UserServiceProvider userServiceProvider = new UserServiceProvider(new InventoryContext(), domainManager);
            LogInService logInService = new LogInService(domainManager, userServiceProvider);
            
            var loginVM = new LoginViewModel(logInService);
            loginVM.LoginCompleted += (sender, args) => {
                if (loginVM.LoginResponce.Success) {
                    this.userService = loginVM.LoginResponce.Service;
                    DXSplashScreen.Show<SETSplashScreen>();
                }
                loginWindow.Close();
            };
            loginWindow.DataContext = loginVM;
            if(DXSplashScreen.IsActive)
                DXSplashScreen.Close();

            loginWindow.ShowDialog();
            return this.userService.IsValid();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (this.userService.IsValid()) {
                this.userService.LogOut();
            }
            base.OnExit(e);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<DXMainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            if (this.userService.IsValid()) {
                var container = containerRegistry.GetContainer();
                container.Register<InventoryContext>(setup: Setup.With(allowDisposableTransient: true));
                container.Register<LocationDataManager>(setup: Setup.With(allowDisposableTransient: true));
                container.Register<UserManager>(setup: Setup.With(allowDisposableTransient: true));
                container.Register<ProductDataManager>(setup: Setup.With(allowDisposableTransient: true));
                container.Register<CategoryDataManager>(setup: Setup.With(allowDisposableTransient: true));

                containerRegistry.Register<ILogInService, LogInService>();
                containerRegistry.Register<IDomainManager, DomainManager>();
                containerRegistry.Register<IEntityDataProvider<Permission>, PermissionProvider>();
                containerRegistry.RegisterInstance<IUserService>(this.userService);
            } else {
                this.Shutdown();
            }
        }

        //protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        //{
        //    switch (this.userService.SoftwareVersion) {
        //        case InventorySoftwareType.MANUFACTURING: {
        //            moduleCatalog.AddModule<FacilityEpiModule>();
        //            moduleCatalog.AddModule<PartsManagmentModule>();
        //            break;
        //        }
        //        case InventorySoftwareType.PRODUCTS_SALES: {
        //            moduleCatalog.AddModule<ProductSalesMainModule>();
        //            moduleCatalog.AddModule<ProductsManagmentModule>();
        //            moduleCatalog.AddModule<ReportingModule>();
        //            break;
        //        }
        //    }
        //    moduleCatalog.AddModule<UserManagmentModule>();
        //    moduleCatalog.AddModule<LocationManagementModule>();
        //    moduleCatalog.AddModule<CategoriesManagmentModule>();
        //}

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) {
            moduleCatalog.AddModule<ProductSalesMainModule>();
            moduleCatalog.AddModule<ProductsManagmentModule>();
            moduleCatalog.AddModule<ReportingModule>();
            moduleCatalog.AddModule<UserManagmentModule>();
            moduleCatalog.AddModule<LocationManagementModule>();
            moduleCatalog.AddModule<CategoriesManagmentModule>();
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            var factory = base.Container.Resolve<IRegionBehaviorFactory>();
            regionAdapterMappings.RegisterMapping(typeof(DocumentGroup), AdapterFactory.Make<RegionAdapterBase<DocumentGroup>>(factory));
            regionAdapterMappings.RegisterMapping(typeof(LayoutPanel), AdapterFactory.Make<RegionAdapterBase<LayoutPanel>>(factory));
            regionAdapterMappings.RegisterMapping(typeof(LayoutGroup), AdapterFactory.Make<RegionAdapterBase<LayoutGroup>>(factory));
            regionAdapterMappings.RegisterMapping(typeof(TabbedGroup), AdapterFactory.Make<RegionAdapterBase<TabbedGroup>>(factory));
        }

        private void ThemeManager_ApplicationThemeChanged(DependencyObject sender, ThemeChangedRoutedEventArgs e)
        {
            ApplicationThemeHelper.SaveApplicationThemeName();
        }
    }
}
