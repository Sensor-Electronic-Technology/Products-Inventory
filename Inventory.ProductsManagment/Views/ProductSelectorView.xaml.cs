using Inventory.Common.EntityLayer.Model.Entities;
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
    /// Interaction logic for ProductSelectorView.xaml
    /// </summary>
    public partial class ProductSelectorView : UserControl {
        public ProductSelectorView() {
            InitializeComponent();
        }

        private void _listOfProducts_CopyingToClipboard(object sender, DevExpress.Xpf.Grid.CopyingToClipboardEventArgs e) {
            var selected = this._listOfProducts.SelectedItem as Product;
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Text, selected.Name);
            e.Handled = true;
        }
    }
}
