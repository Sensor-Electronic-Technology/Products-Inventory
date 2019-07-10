using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Common.EntityLayer.Model.Entities {
    public enum InventorySoftwareType:int {
        [Description("Manufacturing")]  MANUFACTURING,
        [Description("Products/Sales")] PRODUCTS_SALES
    }
}
