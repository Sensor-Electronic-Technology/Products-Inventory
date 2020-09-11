using Inventory.Common.DataLayer.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.Common.BuisnessLayer;
using Inventory.Common.ApplicationLayer;
using Effortless.Net.Encryption;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using System.Text;
using System.Linq;
using Prism.Logging;

namespace Inventory.ApplicationMain.ViewModels {
    public class LoginViewModel : Prism.Mvvm.BindableBase, DevExpress.Mvvm.ISupportServices {
        public DevExpress.Mvvm.IServiceContainer _serviceContainer = null;


        protected DevExpress.Mvvm.IServiceContainer ServiceContainer {
            get {
                if (this._serviceContainer == null) {
                    this._serviceContainer = new DevExpress.Mvvm.ServiceContainer(this);
                }
                return this._serviceContainer;
            }
        }
        DevExpress.Mvvm.IServiceContainer DevExpress.Mvvm.ISupportServices.ServiceContainer { get { return ServiceContainer; } }
        protected IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("Error"); } }
        private ILogInService _loginService;
        private List<string> _versions = new List<string>();
        private string _selectedVersion;

        public event EventHandler LoginCompleted;

        public string Username { get; set; }
        public string Password { get; set; }
        public bool SaveLogin { get; set; }

        public List<string> Versions {
            get => this._versions;
            set => SetProperty(ref this._versions, value, "Versions");
        }

        public string SelectedVersion {
            get => this._selectedVersion;
            set => SetProperty(ref this._selectedVersion, value, "SelectedVersion");
        }

        public LogInResponce LoginResponce { get; set; }
        public Prism.Commands.DelegateCommand LoginCommand { get; private set; }
        public Prism.Commands.DelegateCommand CancelCommand { get; private set; }

        public LoginViewModel(ILogInService loginService)
        {
            this._loginService = loginService;
            this.LoginCommand = new Prism.Commands.DelegateCommand(this.LogIn);
            this.CancelCommand = new Prism.Commands.DelegateCommand(this.Cancel);
            this.PopulateComboBox();
            this.LoadDefault();
        }

        private void PopulateComboBox()
        {
            var temp=new List<string>();
            temp.Add(InventorySoftwareType.MANUFACTURING.GetDescription());
            temp.Add(InventorySoftwareType.PRODUCTS_SALES.GetDescription());
            this.Versions = temp;
            this.SelectedVersion = this.Versions[0];
        }

        private void LoadDefault()
        {
            var saved = Properties.Settings.Default.CredentialsSaved;
            if (saved) {
                this.Username = Properties.Settings.Default.UserName;
                var key = Properties.Settings.Default.userKey;
                var iv = Properties.Settings.Default.userIV;
                var encrypted = Properties.Settings.Default.Password;
                this.Password=Strings.Decrypt(encrypted, key, iv);
                this.SelectedVersion = Properties.Settings.Default.DefaultSoftwareVersion.GetDescription();
                this.SaveLogin = saved;
            }
        }

        private bool CheckProperties(IUserService service)
        {
            if (Properties.Settings.Default.CredentialsSaved) {
                var userName = Properties.Settings.Default.UserName;
                var key = Properties.Settings.Default.userKey;
                var iv = Properties.Settings.Default.userIV;
                var encrypted = Properties.Settings.Default.Password;
                bool credentialsSame=(service.CurrentUser.UserName.Equals(userName) && 
                    service.CurrentUser.EncryptedPassword.Equals(encrypted) && 
                    service.CurrentUser.Key.SequenceEqual(key) && 
                    service.CurrentUser.IV.SequenceEqual(iv));
                return credentialsSame;
            } else {
                return false;
            }
        }

        private void SaveCredentials(IUserService service)
        {
            Properties.Settings.Default.UserName = service.CurrentUser.UserName;
            Properties.Settings.Default.userKey = service.CurrentUser.Key;
            Properties.Settings.Default.userIV = service.CurrentUser.IV;
            Properties.Settings.Default.Password =service.CurrentUser.EncryptedPassword;
            Properties.Settings.Default.CredentialsSaved = true;
            Properties.Settings.Default.DefaultSoftwareVersion = InventorySoftwareType.PRODUCTS_SALES;
            Properties.Settings.Default.Save();
        }

        private void LogIn()
        {
            if (!string.IsNullOrEmpty(this.Username) && !string.IsNullOrEmpty(this.Password) && !string.IsNullOrEmpty(this.SelectedVersion)) {
                //var version = this.SelectedVersion.GetEnum<InventorySoftwareType>(InventorySoftwareType.MANUFACTURING);
                var version = InventorySoftwareType.PRODUCTS_SALES;
                this.LoginResponce = this._loginService.LogInWithPassword(this.Username, this.Password,this.SaveLogin,version);
                if (this.LoginResponce.Success) {
                    if (this.SaveLogin) {
                        if (!this.CheckProperties(this.LoginResponce.Service)) {
                            this.SaveCredentials(this.LoginResponce.Service);
                        }
                    } else {
                        Properties.Settings.Default.Reset();
                    }
                } else {
                    this.ShowErrorMessage(this.LoginResponce);
                }
                this.LoginCompleted?.Invoke(this, EventArgs.Empty);
            } else {
                this.ShowErrorMessage();
            }
        }

        private void ShowErrorMessage(LogInResponce responce=null)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("Error: ");
            if (responce!=null) {
                message.AppendLine("Message: "+responce.Message);
            } else {
                if (string.IsNullOrEmpty(this.Username) && string.IsNullOrEmpty(this.Password)) {
                    message.AppendLine("Username and Password must be filled in to continue");
                } else if (string.IsNullOrEmpty(this.Username)) {
                    message.AppendLine("Username must be filled in to continue");
                } else if (string.IsNullOrEmpty(this.Password)) {
                    message.AppendLine("Password must be filled in to continue");
                }
            }
            this.MessageBoxService.ShowMessage(message.ToString());
        }

        private void Cancel()
        {
            this.LoginResponce = new LogInResponce(null,false);
            this.LoginCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}