namespace Inventory.Common.ApplicationLayer {
    public static class Regions {
        //Main Application
        public static string MainWindow { get { return "MainWindow"; } }
        public static string LoginRegion { get { return "LoginRegion"; } }

        //Facility Module
        public static string MainRegion { get { return "MainRegion"; } }
        public static string ModuleRibbonCommands { get => "ModuleRibbonCommands"; }

        //Parts Module-FacilityMainView
        //public static string EntityRegion { get { return "EntityRegion"; } }
        //public static string CategoryRegion { get { return "CategoryRegion"; } }
        //public static string DetailsRegion { get { return "DetailsRegion"; } }

        //Distributor
        //public static string DistributorEntityRegion { get { return "DistributorEntityRegion"; } }
        //public static string DistributorMainRegion { get { return "DistributorMainRegion"; } }
        //public static string DistributorDetailsRegion { get { return "DistributorDetailsRegion"; } }

        //Product Regions
        public static string ProductMainRegion { get => "ProductMainRegion"; }
        public static string ProductSelectorRegion { get => "ProductSelectorRegion"; }
        public static string ProductDetailsRegion { get => "ProductDetailsRegion"; }
        public static string ProductLotRankRegion { get => "ProductLotRankRegion"; }


        //Location
        public static string LocationMainRegion { get { return "LocationMainRegion"; } }
        public static string LocationTableRegion { get => "LocationTableRegion"; }
        public static string LocationDetailsRegion { get => "LocationDetailsRegion"; }

        public static string LocationWarehouseTable { get => "LocationWarehouseTable"; }
        public static string LocationConsumerTable { get => "LocationConsumerTable"; }
        public static string LocationWarehouseDetails { get => "LocationWarehouseDetails"; }
        public static string LocationConsumerDetails { get => "LocationConsumerDetails"; }

        //Categories
        public static string CategoryMainRegion { get => "CategoryMainRegion"; }
        public static string CategoryTableRegion { get => "CategoryPackageTypeRegion"; }
        public static string CategoryDetailsRegion { get => "PackageTypeDetailsRegion"; }

        //Reports
        public static string ReportsMainRegion { get => "ReportsMainRegion"; }



    //Manufacturer
    //public static string ManufacturerEntityRegion { get { return "ManufacturerEntityRegion"; } }
    //public static string ManufacturerMainRegion { get { return "ManufacturerMainRegion"; } }
    //public static string ManufacturerDetailsRegion { get { return "ManufacturerDetailsRegion"; } }

        //Orders
        //public static string OrdersEntityRegion { get { return "OrdersEntityRegion"; } }
        //public static string OrdersMainRegion { get { return "OrdersMainRegion"; } }
        //public static string OrdersDetailsRegion { get { return "OrdersDetailsRegion"; } }

        //Prices
        //public static string PriceEntityRegion { get { return "PriceEntityRegion"; } }
        //public static string PriceMainRegion { get { return "PriceMainRegion"; } }
        //public static string PriceDetailsRegion { get { return "PriceDetailsRegion"; } }

        //Alerts
        //public static string AlertsEntityRegion { get { return "AlertsEntityRegion"; } }

        //User
        //public static string UserEntityRegion { get { return "UserEntityRegion"; } }
        //public static string UserMainRegion { get { return "UserMainRegion"; } }

        //User Admin
        //public static string UserManagmentRegion { get { return "UserManagmentRegion"; } }
        public static string UserManagmentRegion { get { return "UserManagmentRegion"; } }
        public static string ConfigureNewUserRegion { get { return "ConfigureNewUserRegion"; } }
        public static string ManageExistingUsersRegion { get { return "ManageExistingUsersRegion"; } }
        public static string ExistingUserDetailsRegion { get { return "ExistingUserDetailsRegion"; } }
        public static string UserDetailsRegion { get { return "UserDetailsRegion"; } }

    }
}
