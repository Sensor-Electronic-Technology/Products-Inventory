using System.ComponentModel;

namespace Inventory.Common.EntityLayer.Model.Entities {
    public enum LocationType : int {
        [Description("Consumer")] CONSUMER,
        [Description("Warehouse")] WAREHOUSE
    }
}
