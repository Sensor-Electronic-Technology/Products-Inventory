using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;
using EntityFramework.Triggers;
using System.Linq.Expressions;
using Inventory.Common.ApplicationLayer;
using Inventory.Common.BuisnessLayer;

namespace Inventory.Common.DataLayer.EntityDataManagers {
    public class ManufacturersDataManager : DataManagerBase {

        public ManufacturersDataManager(InventoryContext context, IUserService userService) : base(context, userService) {
        }

        public override void UndoChanges() => throw new NotImplementedException();
    }
}
