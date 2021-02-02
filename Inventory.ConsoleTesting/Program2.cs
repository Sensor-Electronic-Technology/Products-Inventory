using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.Common.DataLayer.EntityDataManagers;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using Inventory.Common.DataLayer.Providers;
using Inventory.Common.BuisnessLayer;
using Inventory.Common.EntityLayer.Model;

namespace Inventory.ConsoleTesting {
    public class ImportLotData {
        public DateTime LotDate { get; set; }
        public string ProductName { get; set; }
        public string LotNumber { get; set; }
        public string Rank { get; set; }
        public int Quantity { get; set; }
        public double UnitCost { get; set; }
        public string PoNumber { get; set; }
    }

    public class Program2 {
        private static void Main(string[] args) {
            IUserService userService;
            DomainManager domainManager = new DomainManager();
            InventoryContext context = new InventoryContext();
            UserServiceProvider userServiceProvider = new UserServiceProvider(context, domainManager);
            LogInService logInService = new LogInService(domainManager, userServiceProvider);
            var responce = logInService.LogInWithPassword("AElmendo", "Drizzle123!", false, InventorySoftwareType.PRODUCTS_SALES);
            userService = responce.Service;
            ProductDataManager dataManager = new ProductDataManager(context,userService);
            ImportService importService = new ImportService(dataManager);
            Console.WriteLine("Starting Service");
            importService.Load();
            Console.WriteLine("Importing Data");
            importService.ImportData();
            importService.Parse();
            Console.WriteLine("Import Finsihed");
            Console.WriteLine("Create Lots");
            var response=importService.Create();
            Console.WriteLine("Should be finished.  See results below:");
            Console.WriteLine(response.Message);
            Console.ReadKey();

        }

        public static void Temp1() {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"C:\ImportLotTemplate.xlsx");
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;


            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;
            List<ImportLotData> data = new List<ImportLotData>();

            for (int i = 2; i <= rowCount; i++) {
                ImportLotData lotData = new ImportLotData();
                double date = double.Parse(xlRange.Cells[i, 1].Value2().ToString());
                lotData.LotDate = DateTime.FromOADate(date);
                lotData.ProductName = xlRange.Cells[i, 2].Value2().ToString();
                lotData.LotNumber = xlRange.Cells[i, 3].Value2().ToString();
                lotData.Rank = xlRange.Cells[i, 4].Value2().ToString();
                lotData.Quantity = int.Parse(xlRange.Cells[i, 5].Value2().ToString());
                lotData.UnitCost = double.Parse(xlRange.Cells[i, 6].Value2().ToString());
                lotData.PoNumber = xlRange.Cells[i, 7].Value2().ToString();
                data.Add(lotData);
            }

            Console.WriteLine("Data:");

            foreach (var item in data) {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t", item.LotDate, item.ProductName, item.ProductName, item.LotNumber, item.Rank
                    , item.Quantity, item.UnitCost, item.PoNumber);

            }

            Console.WriteLine("Done");
            Console.ReadKey();


            GC.Collect();
            GC.WaitForPendingFinalizers();
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
        }

    }

    public class LotCheckinResponse {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ImportLotData LotData { get; set; }

        public LotCheckinResponse(bool success,string message,ImportLotData lotData) {
            this.Success = success;
            this.Message = message;
            this.LotData = lotData;
        }
    }

    public class ImportService {
        private ProductDataManager _dataManager;

        private Excel.Application _excel;
        private Excel.Workbook _workbook;
        private Excel._Worksheet _worksheet;
        private Excel.Range _range;
        private List<ImportLotData> _data;
        private List<LotCheckinResponse> _responseList;
        private List<Lot> _lots;
        private List<ProductInstance> _productInstances;

        public List<LotCheckinResponse> ResponseList { 
            get => this._responseList; 
            set => this._responseList = value;
        }

        public ImportService(ProductDataManager dataManager) {
            this._excel = new Excel.Application();
            this._data = new List<ImportLotData>();
            this._lots = new List<Lot>();
            this._responseList = new List<LotCheckinResponse>();

            this._dataManager = dataManager;
        }

        public InventoryActionResponce Create() {
            return this._dataManager.CheckIn(this._lots);
        }

        public void Parse() {
            foreach(var lotData in this._data) {
                var product=this._dataManager.ProductProvider.GetEntity(lotData.ProductName);          
                if (product != null) {
                    var lotCheck = this._dataManager.LotProvider.GetEntity(e => e.LotNumber == lotData.LotNumber && e.SupplierPoNumber == lotData.PoNumber);
                    if (lotCheck == null) {
                        Cost cost = new Cost();
                        cost.Amount = lotData.UnitCost;
                        Lot lot = new Lot();
                        lot.Cost = cost;
                        lot.LotNumber = lotData.LotNumber;
                        lot.SupplierPoNumber = lotData.PoNumber;
                        lot.Product = product;

                        ProductInstance rank = new ProductInstance();
                        rank.Name = lotData.Rank;
                        rank.Quantity = lotData.Quantity;
                        rank.InventoryItem = product;
                        rank.Lot = lot;
                        rank.InventoryItem = product;
                        lot.ProductInstances.Add(rank);
                        this._lots.Add(lot);
                        this._productInstances.Add(rank);

                    } else {
                        this._responseList.Add(new LotCheckinResponse(false, "Lot Already Exist, Dublicates Not Allowed", lotData));
                    }
                } else {
                    this._responseList.Add(new LotCheckinResponse(false, "Product Not Found", lotData));
                }
            }
        }

        public void ImportData() {
            this._workbook= this._excel.Workbooks.Open(@"C:\ImportLotTemplate.xlsx");
            this._worksheet = this._workbook.Sheets[1];
            this._range= this._worksheet.UsedRange;


            int rowCount = this._range.Rows.Count;
            int colCount = this._range.Columns.Count;

            for (int i = 2; i <= rowCount; i++) {
                ImportLotData lotData = new ImportLotData();
                double date = double.Parse(this._range.Cells[i, 1].Value2().ToString());
                lotData.LotDate = DateTime.FromOADate(date);
                lotData.ProductName = this._range.Cells[i, 2].Value2().ToString();
                lotData.LotNumber = this._range.Cells[i, 3].Value2().ToString();
                lotData.Rank = this._range.Cells[i, 4].Value2().ToString();
                lotData.Quantity = int.Parse(this._range.Cells[i, 5].Value2().ToString());
                lotData.UnitCost = double.Parse(this._range.Cells[i, 6].Value2().ToString());
                lotData.PoNumber = this._range.Cells[i, 7].Value2().ToString();
                this._data.Add(lotData);
            }
        }

        public void CheckOut() {
           
           this._lots.Select(e => e.ProductInstances);

                //this._dataManager.Checkout()

        }
        
        public async Task LoadAsync() {
            await this._dataManager.LoadAsync();
        }

        public void Load() {
            this._dataManager.Load();
        }

        ~ImportService() {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (this._workbook != null) {
                if (this._range != null) {
                    Marshal.ReleaseComObject(this._range);
                }

                if (this._worksheet != null) {
                    Marshal.ReleaseComObject(this._worksheet);
                }
                this._workbook.Close(false);
                Marshal.ReleaseComObject(this._workbook);

            }
            this._excel.Quit();
            Marshal.ReleaseComObject(this._excel);

        }
    }
}
