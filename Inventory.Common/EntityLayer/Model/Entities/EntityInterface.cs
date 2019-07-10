namespace Inventory.Common.EntityLayer.Model.Entities {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EntityFramework.Triggers;

    public interface IEntityWithTracking {
        DateTime? Inserted { get; set; }
        DateTime? Updated { get; set; }
    }
}
