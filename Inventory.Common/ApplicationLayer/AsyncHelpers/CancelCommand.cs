using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.Common.ApplicationLayer.AsyncHelpers {




    public sealed class CancelCommand : ICommand {

        private readonly ICanExecuteChanged _canExecuteChanged;

        private RefCountedCancellationTokenSource _context;

        public CancelCommand(Func<object, ICanExecuteChanged> canExecuteChangedFactory) {
            _canExecuteChanged = canExecuteChangedFactory(this);
        }

        public CancelCommand()
            : this(CanExecuteChangedFactories.DefaultCanExecuteChangedFactory) {
        }

        event EventHandler ICommand.CanExecuteChanged {
            add { _canExecuteChanged.CanExecuteChanged += value; }
            remove { _canExecuteChanged.CanExecuteChanged -= value; }
        }

        bool ICommand.CanExecute(object parameter) {
            return _context != null;
        }

        void ICommand.Execute(object parameter) {
            _context.Cancel();
        }

        private IDisposable StartOperation() {
            if(_context == null) {
                _context = new RefCountedCancellationTokenSource(this);
                _canExecuteChanged.OnCanExecuteChanged();
            }
            return _context.StartOperation();
        }

        private void Notify(RefCountedCancellationTokenSource context) {
            if(_context != context)
                return;
            _context = null;
            _canExecuteChanged.OnCanExecuteChanged();
        }

        public void Cancel() => _context?.Cancel();

        public Func<object, Task> WrapCancel(Func<object, CancellationToken, Task> executeAsync) {
            var wrapped = Wrap(executeAsync);
            return async parameter => {
                Cancel();
                await wrapped(parameter).ConfigureAwait(false);
            };
        }

        public Func<Task> WrapCancel(Func<CancellationToken, Task> executeAsync) {
            var wrapped = Wrap(executeAsync);
            return async () => {
                Cancel();
                await wrapped().ConfigureAwait(false);
            };
        }

        public Func<object, Task> Wrap(Func<object, CancellationToken, Task> executeAsync) {
            return async parameter => {
                using(StartOperation()) {
                    try {
                        await executeAsync(parameter, _context.Token);
                    } catch(OperationCanceledException) {
                    }
                }
            };
        }
        public Func<Task> Wrap(Func<CancellationToken, Task> executeAsync) {
            var wrapped = Wrap((_, token) => executeAsync(token));
            return () => wrapped(null);
        }

        private sealed class RefCountedCancellationTokenSource {
            private readonly CancelCommand _parent;

            private readonly CancellationTokenSource _cts = new CancellationTokenSource();

            private int _count;

            public RefCountedCancellationTokenSource(CancelCommand parent) {
                _parent = parent;
            }

            public CancellationToken Token => _cts.Token;

            private void Signal() {
                if(--_count != 0)
                    return;
                Cancel();
            }

            public IDisposable StartOperation() {
                ++_count;
                return new AnonymousDisposable(Signal);
            }

            public void Cancel() {
                _cts.Cancel();
                _parent.Notify(this);
            }
        }
    }
}

