namespace Inventory.Common.ApplicationLayer {
    using Prism.Commands;

    public interface IApplicationCommands {
        CompositeCommand NewCommand { get; }
        CompositeCommand SaveCommand { get; }
        CompositeCommand DeleteCommand { get; }
    }
}
