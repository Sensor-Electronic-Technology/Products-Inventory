using Inventory.Common.EntityLayer.Model.Entities;

namespace Inventory.Common.BuisnessLayer {
    public interface ILogInService {
        LogInResponce LogInWithPassword(string username, string password, bool storePassword, InventorySoftwareType version);
    }
}