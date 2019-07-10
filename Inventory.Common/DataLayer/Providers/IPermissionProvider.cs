using Inventory.Common.EntityLayer.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Common.DataLayer.Providers {
    public interface IPermissionProvider {
        IList<Permission> GetAvailablePermissions();
    }
}
