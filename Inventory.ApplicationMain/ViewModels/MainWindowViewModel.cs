using Prism.Mvvm;
using DevExpress.Xpf.Core;
using Prism.Commands;

namespace Inventory.ApplicationMain.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public string _title = "Inventory Software";

        public string Title {
            get => this._title;
            set => SetProperty(ref this._title, value, "Title");
        }

        //public DelegateCommand OnLoadedCommand { get; private set; }

        public MainWindowViewModel()
        {
            //this.OnLoadedCommand = new DelegateCommand(this.OnLoadedHandler);
        }


    }
}
