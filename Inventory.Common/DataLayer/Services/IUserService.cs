namespace Inventory.Common.BuisnessLayer {
    using Inventory.Common.EntityLayer.Model;
    using Inventory.Common.EntityLayer.Model.Entities;
    using System.Collections.Generic;

    public enum UserAction {
        Add,
        Edit,
        Remove,
        CheckIn,
        CheckOut,
        UserManagement
    }

    public interface IUserService {
        User CurrentUser { get; set; }
        Session CurrentSession { get; set; }
        Permission UserPermission { get; set; }
        InventorySoftwareType SoftwareVersion { get; set; }
        List<UserAction> AvailableUserActions { get;}
        bool Validate(UserAction action);
        bool IsValid();
        void LogOut();
    }
}
