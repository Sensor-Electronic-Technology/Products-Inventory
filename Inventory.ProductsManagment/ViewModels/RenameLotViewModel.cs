using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ProductsManagment.ViewModels {
    public class RenameLotViewModel {
        public virtual string NewLotNumber { get; set; }
        public virtual string NewSupplierPo { get; set; }
    }
}
