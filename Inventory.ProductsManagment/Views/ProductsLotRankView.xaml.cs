using DevExpress.Xpf.Grid;
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
    /// Interaction logic for ProductsLotRankView.xaml
    /// </summary>
    public partial class ProductsLotRankView : UserControl {
        public ProductsLotRankView() {
            InitializeComponent();
        }

        private void _listOfRanks_CopyingToClipboard(object sender, CopyingToClipboardEventArgs e) {
            var selected = this._listOfRanks.SelectedItem as ProductInstance;
            Clipboard.Clear();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0}\t{1}", selected.LotNumber, selected.Name);
            Clipboard.SetData(DataFormats.Text, builder.ToString());
            e.Handled = true;
        }
    }
}
