namespace Inventory.Common.ApplicationLayer {
    using Prism.Commands;

    public class ApplicationCommands:IApplicationCommands {

        private CompositeCommand _saveCommand = new CompositeCommand();
        private CompositeCommand _deleteCommand = new CompositeCommand();
        private CompositeCommand _newCommand = new CompositeCommand();

        public CompositeCommand SaveCommand {
            get => this._saveCommand;
        }

        public CompositeCommand DeleteCommand {
            get => this._deleteCommand;
        }

        public CompositeCommand NewCommand {
            get => this._newCommand;
        }

    }
}
