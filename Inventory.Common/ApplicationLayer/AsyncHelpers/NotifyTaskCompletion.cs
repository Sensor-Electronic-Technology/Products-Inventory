using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Inventory.Common.ApplicationLayer.AsyncHelpers {
    public sealed class NotifyTask: INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;     
        
        private NotifyTask(Task task) {
            this.Task = task;
            this.TaskCompleted=this.MonitorTaskAsync(task);
        }

        private async Task MonitorTaskAsync(Task task) {
            try {
                await task;
            } catch { } finally {
                this.NotifyProperties(task);
            }
        }

        private void NotifyProperties(Task task) {
            var propertyChanged = this.PropertyChanged;
            if(propertyChanged == null) {
                return;
            }

            if(task.IsCanceled) {
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("Status"));
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("IsCanceled"));
            }else if(task.IsFaulted) {
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("Exception"));
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("InnerException"));
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("ErrorMessage"));
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("Status"));
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("IsFaulted"));
            } else {
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("Status"));
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("IsSuccessfullyCompleted"));
            }
            propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("IsCompleted"));
            propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("IsNotCompleted"));
        }

        public Task Task { get; private set; }
        public Task TaskCompleted { get; private set; }
        public TaskStatus Status { get => this.Task.Status; }
        public bool IsCompleted { get => this.Task.IsCompleted; }
        public bool IsNotCompleted { get => !this.Task.IsCompleted; }
        public bool IsSuccessfullyCompleted { get => this.Task.Status == TaskStatus.RanToCompletion; }
        public bool IsCanceled { get { return Task.IsCanceled; } }
        public bool IsFaulted { get { return Task.IsFaulted; } }
        public AggregateException Exception { get { return Task.Exception; } }
        public Exception InnerException { get { return (Exception == null) ? null : Exception.InnerException; } }
        public string ErrorMessage { get { return (InnerException == null) ? null : InnerException.Message; } }

        public static NotifyTask Create(Task task) {
            return new NotifyTask(task);
        }

        public static NotifyTask<TResult> Create<TResult>(Task<TResult> task, TResult defaultResult = default(TResult)) {
            return new NotifyTask<TResult>(task, defaultResult);
        }

        public static NotifyTask Create(Func<Task> asyncAction) {
            return Create(asyncAction());
        }

        public static NotifyTask<TResult> Create<TResult>(Func<Task<TResult>> asyncAction, TResult defaultResult = default(TResult)) {
            return Create(asyncAction(), defaultResult);
        }
    }

    public sealed class NotifyTask<TResult> : INotifyPropertyChanged {
        private readonly TResult _defaultResult;
        public event PropertyChangedEventHandler PropertyChanged;

        internal NotifyTask(Task<TResult> task,TResult defaultResult) {
            this._defaultResult = defaultResult;
            this.Task = task;
            this.TaskCompleted = this.MonitorTaskAsync(task);
        }

        private async Task MonitorTaskAsync(Task task) {
            try {
                await task;
            } catch {} finally {
                NotifyProperties(task);
            }
        }

        private void NotifyProperties(Task task) {
            var propertyChanged = PropertyChanged;
            if(propertyChanged == null)
                return;

            if(task.IsCanceled) {
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("Status"));
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("IsCanceled"));
            } else if(task.IsFaulted) {
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("Exception"));
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("InnerException"));
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("ErrorMessage"));
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("Status"));
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("IsFaulted"));
            } else {
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("Result"));
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("Status"));
                propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("IsSuccessfullyCompleted"));
            }
            propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("IsCompleted"));
            propertyChanged(this, PropertyChangedEventArgsCache.Instance.Get("IsNotCompleted"));
        }

        public Task<TResult> Task { get; private set; }
        public Task TaskCompleted { get; private set; }
        public TResult Result { get { return (this.Task.Status == TaskStatus.RanToCompletion) ? Task.Result : _defaultResult; } }
        public TaskStatus Status { get { return this.Task.Status; } }
        public bool IsCompleted { get { return this.Task.IsCompleted; } }
        public bool IsNotCompleted { get { return !this.Task.IsCompleted; } }
        public bool IsSuccessfullyCompleted { get { return this.Task.Status == TaskStatus.RanToCompletion; } }
        public bool IsCanceled { get { return this.Task.IsCanceled; } }
        public bool IsFaulted { get { return this.Task.IsFaulted; } }
        public AggregateException Exception { get { return this.Task.Exception; } }
        public Exception InnerException { get { return (this.Exception == null) ? null : this.Exception.InnerException; } }
        public string ErrorMessage { get { return (this.InnerException == null) ? null : this.InnerException.Message; } }

    }



}
