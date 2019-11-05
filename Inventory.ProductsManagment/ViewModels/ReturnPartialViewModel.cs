using System;
using DevExpress.Mvvm;
using Inventory.Common.EntityLayer.Model.Entities;

namespace Inventory.ProductsManagment.ViewModels {
    public class ReturnPartialViewModel : ViewModelBase {
        public virtual ProductInstance SelectedRank { get; set; }
        public virtual int NewQuantity { get; set; }
        public virtual string BuyerPo { get; set; }
        public virtual string RMA { get; set; }
    }
}