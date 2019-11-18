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
            if (transaction.InventoryAction == InventoryAction.OUTGOING) {
                this.Transaction.Quantity *= -1;

            }

            this.Cost = this.UnitCost * this.Transaction.Quantity;
            
            //this.Cost = (transaction.TotalCost != null) ? transaction.TotalCost.Value : 0;
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

    public class ReportSummaryRow {
        public double QtyBeginning { get; set; }
        public double QtyPurchases { get; set; }
        public double QtyReturns { get; set; }
        public double ConsumedCustomer{ get; set; }
        public double ConsumedRnD { get; set; }
        public double Rejected { get; set; }
        public double Obsoleted { get; set; }
        public double TotalConsumed { get; set; }
        public double QtyEnding { get; set; }
    }

    public interface IProductAction {
        double Quantity { get; set; }
        double Cost { get; set; }
    }

    public class ProductIncoming : IProductAction {
        public double Quantity { get; set; }
        public double Cost { get; set; }
    }

    public class Return : IProductAction {
        public double Quantity { get; set; }
        public double Cost { get; set; }
    }

    public class Customer : IProductAction {
        public double Quantity { get; set; }
        public double Cost { get; set; }
    }

    public class Internal : IProductAction {
        public double Quantity { get; set; }
        public double Cost { get; set; }
    }

    public class QualityScrap : IProductAction {
        public double Quantity { get; set; }
        public double Cost { get; set; }
    }

    public class ProductSnapshot {

        public ProductSnapshot() {
            this.ProductIncoming = new ProductIncoming();
            this.Return = new Return();
            this.QualityScrap = new QualityScrap();
            this.Internal = new Internal();
            this.Customer = new Customer();
        }

        public string ProductName { get; set; }

        public double QtyStart { get; set; }
        public double CostStart { get; set; }


        //Incoming
        public ProductIncoming ProductIncoming { get; set; }
        public Return Return { get; set; }

        //Outgoing
        public QualityScrap QualityScrap { get; set; }
        public Internal Internal { get; set; }
        public Customer Customer { get; set; }

        public double QtyEnd { get; set; }
        public double CostEnd { get; set; }

        public double QtyCurrent { get; set; }
        public double CostCurrent { get; set; }
    }


    public class ProductCostSnapshot {
        public string ProductName { get; set; }
        
        public double QtyStart { get; set; }
        public double CostStart { get; set; }
        public double QtyIncoming { get; set; }
        public double CostIncoming { get; set; }
        public double QtyOutgoing { get; set; }
        public double CostOutgoing { get; set; }
        public double QtyEnd { get; set; }
        public double CostEnd { get; set; }
        public double QtyCurrent { get; set; }
        public double CostCurrent { get; set; }
    }
}
