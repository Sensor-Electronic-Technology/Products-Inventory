using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Common.ApplicationLayer {

    public enum InventoryOperations {
        [Description("Delete")] DELETE,
        [Description("Update")] UPDATE,
        [Description("Add")] ADD
    }

    //public static class InventoryOperations {
    //    public static string Delete { get => "Delete"; }
    //    public static string Update { get => "Update"; }
    //    public static string Add { get => "Add"; }
    //}
}
