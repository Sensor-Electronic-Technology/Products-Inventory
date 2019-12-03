namespace Inventory.Common.ApplicationLayer {
    public static class Regions {
        //Main Application
        public static string MainWindow { get { return "MainWindow"; } }
        public static string LoginRegion { get { return "LoginRegion"; } }

        //Facility Module
        public static string MainRegion { get { return "MainRegion"; } }
        public static string ModuleRibbonCommands { get => "ModuleRibbonCommands"; }


        //Product Regions
        public static string ProductMainRegion { get => "ProductMainRegion"; }
        public static string ProductSelectorRegion { get => "ProductSelectorRegion"; }
        public static string ProductDetailsRegion { get => "ProductDetailsRegion"; }
        public static string ProductLotRankRegion { get => "ProductLotRankRegion"; }

        //Part Regions
        public static string PartMainRegion { get => "PartMainRegion"; }
        public static string PartSelectorRegion { get => "PartSelectorRegion"; }
        public static string PartDetailsRegion { get => "PartDetailsRegion"; }
        public static string InstanceDetailsRegion { get => "InstanceDetailsRegion"; }

        //Location
        public static string LocationMainRegion { get =>"LocationMainRegion"; }
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

        public static string UserManagmentRegion { get { return "UserManagmentRegion"; } }
        public static string ConfigureNewUserRegion { get { return "ConfigureNewUserRegion"; } }
        public static string ManageExistingUsersRegion { get { return "ManageExistingUsersRegion"; } }
        public static string ExistingUserDetailsRegion { get { return "ExistingUserDetailsRegion"; } }
        public static string UserDetailsRegion { get { return "UserDetailsRegion"; } }

    }
}
