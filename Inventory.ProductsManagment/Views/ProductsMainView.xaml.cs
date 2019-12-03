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

namespace Inventory.ProductsManagment.Views {
    /// <summary>
    /// Interaction logic for ProductsMainView.xaml
    /// </summary>
    public partial class ProductsMainView : UserControl {
        private bool _isactive;
        public string PanelCaption { get { return "Parts Managment"; } }

        public bool IsActive {
            get => this._isactive;
            set => this._isactive = value;
        }

        public ProductsMainView() {
            InitializeComponent();
            this.IsActive = true;
        }
    }
}
