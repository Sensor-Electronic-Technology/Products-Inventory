using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Common.ApplicationLayer.AsyncHelpers {
    public interface ICanExecuteChanged {
        event EventHandler CanExecuteChanged;
        void OnCanExecuteChanged();
    }

    public interface ICanExecute {
        event EventHandler CanExecuteChanged;
        bool CanExecute(object parameter);
    }

    public static class CanExecuteChangedFactories {

        public static readonly Func<object, ICanExecuteChanged> WeakCanExecuteChangedFactory = sender => new WeakCanExecuteChanged(sender);

        public static readonly Func<object, ICanExecuteChanged> StrongCanExecuteChangedFactory = sender => new StrongCanExecuteChanged(sender);

        public static Func<object, ICanExecuteChanged> DefaultCanExecuteChangedFactory { get; set; } = StrongCanExecuteChangedFactory;
    }

    public sealed class WeakCanExecuteChanged : ICanExecuteChanged {

        private readonly object _sender;
        private readonly WeakCollection<EventHandler> _canExecuteChanged = new WeakCollection<EventHandler>();
        private ThreadAffinity _threadAffinity;

        public WeakCanExecuteChanged(object sender) {
            _threadAffinity = ThreadAffinity.BindToCurrentThread();
            _sender = sender;
        }

        public event EventHandler CanExecuteChanged {
            add {
                _threadAffinity.VerifyCurrentThread();
                _canExecuteChanged.Add(value);
            }
            remove {
                _threadAffinity.VerifyCurrentThread();
                _canExecuteChanged.Remove(value);
            }
        }

        public void OnCanExecuteChanged() {
            _threadAffinity.VerifyCurrentThread();
            foreach(var canExecuteChanged in _canExecuteChanged.GetLiveItems())
                canExecuteChanged(_sender, EventArgs.Empty);
        }
    }

    public sealed class StrongCanExecuteChanged : ICanExecuteChanged {

        private readonly object _sender;

        private event EventHandler _canExecuteChanged;

        private ThreadAffinity _threadAffinity;

        public StrongCanExecuteChanged(object sender) {
            _threadAffinity = ThreadAffinity.BindToCurrentThread();
            _sender = sender;
        }

        public event EventHandler CanExecuteChanged {
            add {
                _threadAffinity.VerifyCurrentThread();
                _canExecuteChanged += value;
            }
            remove {
                _threadAffinity.VerifyCurrentThread();
                _canExecuteChanged -= value;
            }
        }

        public void OnCanExecuteChanged() {
            _threadAffinity.VerifyCurrentThread();
            _canExecuteChanged?.Invoke(_sender, EventArgs.Empty);
        }
    }
}
