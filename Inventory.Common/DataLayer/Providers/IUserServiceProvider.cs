using Inventory.Common.DataLayer.Services;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.Common.BuisnessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Common.DataLayer.Providers {
    public interface IUserServiceProvider {
        IUserService CreateUserServiceUserAuthenticated(User user, InventorySoftwareType version, bool storePassword, string permission = null, string password = null);
        IUserService CreateUserServiceNoPermissions(User user, InventorySoftwareType version, bool storePassword, string password = null);
    }
}
