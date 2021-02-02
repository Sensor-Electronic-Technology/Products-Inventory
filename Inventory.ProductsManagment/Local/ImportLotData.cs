using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ProductsManagment.Local {
    public class ImportLotData {
        public DateTime LotDate { get; set; }
        public string ProductName { get; set; }
        public string LotNumber { get; set; }
        public string Rank { get; set; }
        public int Quantity { get; set; }
        public double UnitCost { get; set; }
        public string PoNumber { get; set; }
    }
}
