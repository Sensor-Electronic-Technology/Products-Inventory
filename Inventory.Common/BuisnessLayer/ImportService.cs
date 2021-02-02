using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Common.ApplicationLayer;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;
using Excel = Microsoft.Office.Interop.Excel;

namespace Inventory.Common.BuisnessLayer {
    
    
    public class ImportService {
        Excel.Application xlApp;

        public ImportService() {
            this.xlApp=new Excel.Application();
        }

        public void ImportData() {
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"sandbox_test.xlsx");
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;
            
        }




    }
}
