using Inventory.Common.EntityLayer.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Common.BuisnessLayer {
    public class ReportDataRow {
        public ProductTransaction Transaction { get; set; }
        public string ProductName { get; set; }
        public string Vendor { get; set; }
        public string LotNumber { get; set; }
        public string SupplierPoNumber { get; set; }
        public double UnitCost { get; set; }
        public double Cost { get; set; }

        public ReportDataRow(ProductTransaction transaction,Lot lot) {
            this.Transaction = transaction;
            this.ProductName = transaction.Instance.InventoryItem.Name;
            this.Vendor = lot.Cost.DistributorName;
            this.LotNumber = lot.LotNumber;
            this.SupplierPoNumber = lot.SupplierPoNumber;
            this.UnitCost = (transaction.UnitCost != null) ? transaction.UnitCost.Value : 0;
            this.Cost =(transaction.TotalCost!=null) ? transaction.TotalCost.Value:0;
        }
    }

    public class TotalReportDataRow {
        public Product Product { get; set; }
        public double TotalCost { get; set; }
    }

    public class ReportDataRow_v1 {

        public string Category { get; set; }
        public string PartNumber { get; set; }
        public string Vendor { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime TimeOut { get; set; }
        public double UnitCost { get; set; }
        public double QuantityStart { get; set; }
        public double QuantityPurchased { get; set; }
        public double QuantityConsumed { get; set; }
        public string Location { get; set; }
        public double QuantityReturned { get; set; }
        public string BuyerPoNumber { get; set; }
        public string RMANumber { get; set; }

    }

    public class ProductCostRow {
        public string Rank { get; set; }
        public double Quantity { get; set; }
        public double UnitCost { get; set; }
        public double TotalCost { get; set; }

        public ProductCostRow(ProductInstance rank,double unitCost) {
            this.Rank = rank.Name;
            this.Quantity = rank.Quantity;
            this.UnitCost = unitCost;
            this.TotalCost = unitCost * this.Quantity;
        }

    }
}
