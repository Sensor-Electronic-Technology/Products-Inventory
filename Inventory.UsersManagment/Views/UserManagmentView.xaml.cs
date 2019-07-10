using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inventory.UsersManagment.Views {
    /// <summary>
    /// Interaction logic for UserManagmentView.xaml
    /// </summary>
    public partial class UserManagmentView : UserControl {
        private bool _isactive;
        public string PanelCaption { get { return "User Managment Module"; } }

        public bool IsActive {
            get => this._isactive;
            set => this._isactive = value;
        }

        public UserManagmentView() {
            InitializeComponent();
            this.IsActive = true;
        }
    }
}
