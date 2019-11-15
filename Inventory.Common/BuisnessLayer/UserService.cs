using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Common.ApplicationLayer;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;

namespace Inventory.Common.BuisnessLayer {
    public class UserService : IUserService {
        public User CurrentUser { get; set; }
        public Session CurrentSession { get; set; }
        public Permission UserPermission { get; set; }
        public InventorySoftwareType SoftwareVersion { get; set; }
        public List<UserAction> AvailableUserActions { get; private set; }

        public UserService(User currentUser, Session currentSession, Permission userPermission, InventorySoftwareType softwareVersion)
        {
            this.CurrentUser = currentUser;
            this.CurrentSession = currentSession;
            this.UserPermission = userPermission;
            this.SoftwareVersion = softwareVersion;
            this.Initialize();
        }

        public UserService()
        {
            this.CurrentUser = null;
            this.CurrentSession = null;
            this.UserPermission = null;
        }

        public bool IsValid()
        {
            return (this.CurrentUser != null && this.CurrentSession != null);
        }

        private void Initialize() {
            this.AvailableUserActions = new List<UserAction>();
            if(this.UserPermission.Name == "InventoryAdminAccount") {
                this.AvailableUserActions.Add(UserAction.Add);
                this.AvailableUserActions.Add(UserAction.Edit);
                this.AvailableUserActions.Add(UserAction.Remove);
                this.AvailableUserActions.Add(UserAction.CheckIn);
                this.AvailableUserActions.Add(UserAction.CheckOut);
                this.AvailableUserActions.Add(UserAction.UserManagement);

            } else if(this.UserPermission.Name == "InventoryUserAccount") {
                this.AvailableUserActions.Add(UserAction.CheckIn);
                this.AvailableUserActions.Add(UserAction.CheckOut);

            } else if(this.UserPermission.Name == "InventoryUserFullAccount") {
                this.AvailableUserActions.Add(UserAction.Edit);
                this.AvailableUserActions.Add(UserAction.Remove);
                this.AvailableUserActions.Add(UserAction.CheckIn);
                this.AvailableUserActions.Add(UserAction.CheckOut);
            }
        }

        public bool Validate(UserAction action) {
            if (this.AvailableUserActions == null)
                this.Initialize();

            return this.AvailableUserActions.Contains(action);
        }

        public void LogOut()
        {
            using(var context=new InventoryContext()) {
                var entry=context.Entry<Session>(this.CurrentSession);
                entry.Entity.Out = DateTime.UtcNow;
                entry.State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}
