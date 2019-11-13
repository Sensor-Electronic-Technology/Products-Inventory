namespace Inventory.Common.ApplicationLayer {
    public static class AppViews {
        public static string LoginView { get { return "LoginView"; } }

        //Main Application
        public static string MainWindowView { get { return "MainWindow"; } }
        public static string WelcomeView { get => "WelcomeView"; }

        //Facility Module
        public static string FacilityMainView { get { return "FacilityMainView"; } }

        //Parts Module
        public static string PartsDetailsView { get { return "PartsDetailsView"; } }
        public static string PartsCategoryView { get { return "PartsCategoryView"; } }
        public static string PartsMainView { get { return "PartsMainView"; } }
        public static string PartsTableDetailsView { get { return "PartsTableDetailsView"; } }

        //Products Module
        public static string ProductReservationView { get => "ProductReservationView"; }
        public static string LotDetailsView { get => "LotDetailsView"; }
        public static string RankDetailsView { get => "RankDetailsView"; }
        public static string ProductsMainView { get => "ProductsMainView"; }
        public static string ProductsLotRankView { get => "ProductsLotRankView"; }
        public static string ProductSelectorView { get => "ProductSelectorView"; }
        public static string OutgoingProductListView { get => "OutgoingProductListView"; }
        public static string IncomingProductListView { get => "IncomingProductListView"; }
        public static string IncomingProductFormView { get => "IncomingProductFormView"; }
        public static string IncomingInstructionsView { get => "IncomingInstructionsView"; }

        //Location Module
        public static string WarehouseTableView { get { return "WarehouseTableView"; } }
        public static string WarehouseDetailsView { get { return "WarehouseDetailsView"; } }
        public static string ConsumerDetailsView { get { return "ConsumerDetailsView"; } }
        public static string ConsumerTableView { get { return "ConsumerTableView"; } }
        public static string LocationMainView { get { return "LocationMainView"; } }
        public static string LocationDetailsView { get { return "LocationDetailsView"; } }

        //User Module
        public static string UserManagmentView { get { return "UserManagmentView"; } }
        public static string UserDetailsView { get { return "UserDetailsView"; } }
        public static string UserMainView { get { return "UserMainView"; } }

        public static string ConfigureNewUserView { get => "ConfigureNewUserView"; }
        public static string ManageExistingUsersView { get => "ManageExistingUsersView"; }

        //Category Module
        public static string ProductTypeTableView { get => "ProductTypeTableView"; }
        public static string CategoryMainView { get => "CategoryMainView"; }
        public static string CategoryDetailsView { get => "CategoryDetailsView"; }

        //Report Module
        public static string ReportingMainView { get => "ReportingMainView"; }
        public static string ReportsMainView { get => "ReportsMainView"; }
        public static string ReportsSnapshotView { get => "ReportsSnapshotView"; }
        public static string ReportsTransactionSummaryView { get => "ReportsTransactionSummaryView"; }
        public static string ReportsCurrentInventoryView { get => "ReportsCurrentInventoryView"; }

    }
}
