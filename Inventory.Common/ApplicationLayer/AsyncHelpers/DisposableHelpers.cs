using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Common.ApplicationLayer.AsyncHelpers {
    public abstract class SingleDisposable<T> : IDisposable {

        private readonly BoundActionField<T> _context;

        private readonly ManualResetEventSlim _mre = new ManualResetEventSlim();

        protected SingleDisposable(T context) {
            _context = new BoundActionField<T>(Dispose, context);
        }

        public bool IsDisposeStarted => _context.IsEmpty;

        public bool IsDisposed => _mre.IsSet;

        public bool IsDisposing => IsDisposeStarted && !IsDisposed;


        protected abstract void Dispose(T context);


        public void Dispose() {
            var context = _context.TryGetAndUnset();
            if(context == null) {
                _mre.Wait();
                return;
            }
            try {
                context.Invoke();
            } finally {
                _mre.Set();
            }
        }
        protected bool TryUpdateContext(Func<T, T> contextUpdater) => _context.TryUpdateContext(contextUpdater);
    }

    public sealed class AnonymousDisposable : SingleDisposable<Action> {

        public AnonymousDisposable(Action dispose)
            : base(dispose) {
        }
        protected override void Dispose(Action context) => context?.Invoke();

        public void Add(Action dispose) {
            if(dispose == null)
                return;
            if(!TryUpdateContext(x => x + dispose))
                dispose();
        }
        public static AnonymousDisposable Create(Action dispose) => new AnonymousDisposable(dispose);
    }
}
