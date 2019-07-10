using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.Common.ApplicationLayer.AsyncHelpers {

    public interface IAsyncCommand : ICommand {
        Task ExecuteAsync(object parameter);
    }

    public abstract class AsyncCommandBase : IAsyncCommand {
        private readonly ICanExecuteChanged _canExecuteChanged;
        protected AsyncCommandBase(Func<object, ICanExecuteChanged> canExecuteChangedFactory) {
            this._canExecuteChanged = canExecuteChangedFactory(this);
        }

        public abstract Task ExecuteAsync(object parameter);
        protected abstract bool CanExecute(object parameter);

        protected void OnCanExecuteChanged() {
            this._canExecuteChanged.OnCanExecuteChanged();
        }

        event EventHandler ICommand.CanExecuteChanged {
            add { this._canExecuteChanged.CanExecuteChanged += value; }
            remove { this._canExecuteChanged.CanExecuteChanged -= value; }
        }

        bool ICommand.CanExecute(object parameter) {
            return this.CanExecute(parameter);
        }

        async void ICommand.Execute(object parameter) {
            await this.ExecuteAsync(parameter);
        }
    }

    public sealed class AsyncCommand : AsyncCommandBase, INotifyPropertyChanged {
        private readonly Func<object, Task> _executeAsync;

        public AsyncCommand(Func<object, Task> executeAsync, Func<object, ICanExecuteChanged> canExecuteChangedFactory)
            : base(canExecuteChangedFactory) {
            _executeAsync = executeAsync;
        }

        public AsyncCommand(Func<object, Task> executeAsync)
            : this(executeAsync, CanExecuteChangedFactories.DefaultCanExecuteChangedFactory) {
        }

        public AsyncCommand(Func<Task> executeAsync, Func<object, ICanExecuteChanged> canExecuteChangedFactory)
            : this(_ => executeAsync(), canExecuteChangedFactory) {
        }

        public AsyncCommand(Func<Task> executeAsync)
            : this(_ => executeAsync(), CanExecuteChangedFactories.DefaultCanExecuteChangedFactory) {
        }



        public NotifyTask Execution { get; private set; }

        public bool IsExecuting {
            get {
                if(Execution == null)
                    return false;
                return Execution.IsNotCompleted;
            }
        }

        public override async Task ExecuteAsync(object parameter) {
            var tcs = new TaskCompletionSource<object>();
            Execution = NotifyTask.Create(DoExecuteAsync(tcs.Task, _executeAsync, parameter));
            OnCanExecuteChanged();
            var propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("Execution"));
            propertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("IsExecuting"));
            tcs.SetResult(null);
            await Execution.TaskCompleted;
            OnCanExecuteChanged();
            PropertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("IsExecuting"));
            await Execution.Task;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected override bool CanExecute(object parameter) => !IsExecuting;

        private static async Task DoExecuteAsync(Task precondition, Func<object, Task> executeAsync, object parameter) {
            await precondition;
            await executeAsync(parameter);
        }
    }
}
