using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Common.ApplicationLayer.AsyncHelpers {
    public sealed class CustomAsyncCommand : AsyncCommandBase, INotifyPropertyChanged {
        private readonly Func<object, Task> _executeAsync;
        private readonly Func<object, bool> _canExecute;

        public CustomAsyncCommand(Func<object, Task> executeAsync, Func<object, bool> canExecute, Func<object, ICanExecuteChanged> canExecuteChangedFactory)
            : base(canExecuteChangedFactory) {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }

        public CustomAsyncCommand(Func<object, Task> executeAsync, Func<object, bool> canExecute)
            : this(executeAsync, canExecute, CanExecuteChangedFactories.DefaultCanExecuteChangedFactory) {
        }

        public CustomAsyncCommand(Func<Task> executeAsync, Func<bool> canExecute, Func<object, ICanExecuteChanged> canExecuteChangedFactory)
            : this(_ => executeAsync(), _ => canExecute(), canExecuteChangedFactory) {
        }

        public CustomAsyncCommand(Func<Task> executeAsync, Func<bool> canExecute)
            : this(_ => executeAsync(), _ => canExecute(), CanExecuteChangedFactories.DefaultCanExecuteChangedFactory) {
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
            var propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("Execution"));
            propertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("IsExecuting"));
            tcs.SetResult(null);
            await Execution.TaskCompleted;
            PropertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("IsExecuting"));
            await Execution.Task;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected override bool CanExecute(object parameter) => _canExecute(parameter);
        public new void OnCanExecuteChanged() => base.OnCanExecuteChanged();
        private static async Task DoExecuteAsync(Task precondition, Func<object, Task> executeAsync, object parameter) {
            await precondition;
            await executeAsync(parameter);
        }
    }
}
