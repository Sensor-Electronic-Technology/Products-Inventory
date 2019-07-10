using DevExpress.Xpf.Grid;
using System;
using System.Windows;
using DevExpress.Mvvm.Xpf;
using DevExpress.Mvvm.UI;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;

namespace Inventory.Common.ApplicationLayer.UI_Services {

    public interface IControlUpdateService {
        void BeginUpdate();
        void EndUpdate();
    }

    public interface IGridUpdateService : IControlUpdateService {

    }

    public interface IComboBoxEditUpdateService:IGridUpdateService {
    }

    public class GridUpdateService : ServiceBase, IGridUpdateService {
        public static readonly DependencyProperty GridControlProperty=
            DependencyProperty.Register("GridControl", typeof(GridControl), typeof(GridUpdateService), new PropertyMetadata(null));

        public GridControl GridControl {
            get { return (GridControl)GetValue(GridControlProperty); }
            set { SetValue(GridControlProperty, value); }
        }

        protected GridControl ActualGridControl { get { return GridControl != null ? GridControl : AssociatedObject as GridControl; } }

        public void BeginUpdate() {
            Dispatcher.Invoke(new Action(() => {
                if(ActualGridControl != null) {
                    ActualGridControl.ShowLoadingPanel = true;
                    ActualGridControl.BeginDataUpdate();
                }
            }));
        }

        public void EndUpdate() {
            Dispatcher.Invoke(new Action(() => {
                if(ActualGridControl != null) {
                    ActualGridControl.ShowLoadingPanel = false;
                    ActualGridControl.EndDataUpdate();
                }
            }));
        }
    }

    public class ComboBoxEditUpdateService : ServiceBase, IComboBoxEditUpdateService {
        public static readonly DependencyProperty ComboBoxEditProperty =
            DependencyProperty.Register("ComboBoxEdit", typeof(ComboBoxEdit), typeof(GridUpdateService), new PropertyMetadata(null));

        public ComboBoxEdit ComboBoxEdit {
            get { return (ComboBoxEdit)GetValue(ComboBoxEditProperty); }
            set { SetValue(ComboBoxEditProperty, value); }
        }

        protected ComboBoxEdit ActualComboBoxEdit{ get { return ComboBoxEdit != null ? ComboBoxEdit : AssociatedObject as ComboBoxEdit; } }

        public void BeginUpdate() {
            Dispatcher.Invoke(new Action(() => {
                if(ActualComboBoxEdit != null) {
                    ActualComboBoxEdit.IsEnabled = false;
                    ActualComboBoxEdit.BeginDataUpdate();
                }
            }));
        }

        public void EndUpdate() {
            Dispatcher.Invoke(new Action(() => {
                if(ActualComboBoxEdit != null) {
                    ActualComboBoxEdit.IsEnabled = true;
                    ActualComboBoxEdit.EndDataUpdate();
                }
            }));
        }
    }
}
