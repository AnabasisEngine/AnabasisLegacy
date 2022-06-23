using System.Runtime.ExceptionServices;

namespace Anabasis.Tasks;

public readonly partial struct AnabasisTask
{
    private static readonly AnabasisTask CanceledAnabasisTask =
        new Func<AnabasisTask>(() => new AnabasisTask(new CanceledResultSource(CancellationToken.None), 0))();

    private static class CanceledAnabasisTaskCache<T>
    {
        public static readonly AnabasisTask<T> Task;

        static CanceledAnabasisTaskCache() {
            Task = new AnabasisTask<T>(new CanceledResultSource<T>(CancellationToken.None), 0);
        }
    }

    public static readonly AnabasisTask CompletedTask;

    public static AnabasisTask FromException(Exception ex) =>
        ex is OperationCanceledException oce
            ? FromCanceled(oce.CancellationToken)
            : new AnabasisTask(new ExceptionResultSource(ex), 0);

    public static AnabasisTask<T> FromException<T>(Exception ex) =>
        ex is OperationCanceledException oce
            ? FromCanceled<T>(oce.CancellationToken)
            : new AnabasisTask<T>(new ExceptionResultSource<T>(ex), 0);

    public static AnabasisTask<T> FromResult<T>(T value) => new(value);

    public static AnabasisTask FromCanceled(CancellationToken cancellationToken = default) => cancellationToken == CancellationToken.None ? CanceledAnabasisTask : new AnabasisTask(new CanceledResultSource(cancellationToken), 0);

    public static AnabasisTask<T> FromCanceled<T>(CancellationToken cancellationToken = default) => cancellationToken == CancellationToken.None ? CanceledAnabasisTaskCache<T>.Task : new AnabasisTask<T>(new CanceledResultSource<T>(cancellationToken), 0);

    public static AnabasisTask Create(Func<AnabasisTask> factory) => factory();

    public static AnabasisTask<T> Create<T>(Func<AnabasisTask<T>> factory) => factory();

    // public static AsyncLazy Lazy(Func<AnabasisTask> factory) => new AsyncLazy(factory);

    // public static AsyncLazy<T> Lazy<T>(Func<AnabasisTask<T>> factory) => new AsyncLazy<T>(factory);

    /// <summary>
    /// helper of fire and forget void action.
    /// </summary>
    public static void Void(Func<AnabasisVoidTask> asyncAction) {
        asyncAction().Forget();
    }

    /// <summary>
    /// helper of fire and forget void action.
    /// </summary>
    public static void Void(Func<CancellationToken, AnabasisVoidTask> asyncAction,
        CancellationToken cancellationToken) {
        asyncAction(cancellationToken).Forget();
    }

    /// <summary>
    /// helper of fire and forget void action.
    /// </summary>
    public static void Void<T>(Func<T, AnabasisVoidTask> asyncAction, T state) {
        asyncAction(state).Forget();
    }

    /// <summary>
    /// helper of create add AnabasisTaskVoid to delegate.
    /// For example: FooAction = AnabasisTask.Action(async () => { /* */ })
    /// </summary>
    public static Action Action(Func<AnabasisVoidTask> asyncAction) => () => asyncAction().Forget();

    /// <summary>
    /// helper of create add AnabasisTaskVoid to delegate.
    /// </summary>
    public static Action Action(Func<CancellationToken, AnabasisVoidTask> asyncAction,
        CancellationToken cancellationToken) =>
        () => asyncAction(cancellationToken).Forget();

    /// <summary>
    /// Defer the task creation just before call await.
    /// </summary>
    public static AnabasisTask Defer(Func<AnabasisTask> factory) => new(new DeferPromise(factory), 0);

    /// <summary>
    /// Defer the task creation just before call await.
    /// </summary>
    public static AnabasisTask<T> Defer<T>(Func<AnabasisTask<T>> factory) =>
        new(new DeferPromise<T>(factory), 0);

    /// <summary>
    /// Never complete.
    /// </summary>
    public static AnabasisTask Never(CancellationToken cancellationToken) => new AnabasisTask<AsyncUnit>(new NeverPromise<AsyncUnit>(cancellationToken), 0);

    /// <summary>
    /// Never complete.
    /// </summary>
    public static AnabasisTask<T> Never<T>(CancellationToken cancellationToken) => new(new NeverPromise<T>(cancellationToken), 0);

    private sealed class ExceptionResultSource : IAnabasisTaskSource
    {
        private readonly ExceptionDispatchInfo _exception;
        private          bool                  _calledGet;

        public ExceptionResultSource(Exception exception) {
            _exception = ExceptionDispatchInfo.Capture(exception);
        }

        public void GetResult(short token) {
            if (!_calledGet) {
                _calledGet = true;
                GC.SuppressFinalize(this);
            }

            _exception.Throw();
        }

        public AnabasisTaskStatus GetStatus(short token) => AnabasisTaskStatus.Faulted;

        public AnabasisTaskStatus UnsafeGetStatus() => AnabasisTaskStatus.Faulted;

        public void OnCompleted(Action<object?> continuation, object? state, short token) {
            continuation(state);
        }

        ~ExceptionResultSource() {
            if (!_calledGet) {
                AnabasisTaskScheduler.PublishUnobservedTaskException(_exception.SourceException);
            }
        }
    }

    private sealed class ExceptionResultSource<T> : IAnabasisTaskSource<T>
    {
        private readonly ExceptionDispatchInfo _exception;
        private          bool                  _calledGet;

        public ExceptionResultSource(Exception exception) {
            _exception = ExceptionDispatchInfo.Capture(exception);
        }

        public T GetResult(short token) {
            if (!_calledGet) {
                _calledGet = true;
                GC.SuppressFinalize(this);
            }

            _exception.Throw();
            return default;
        }

        void IAnabasisTaskSource.GetResult(short token) {
            if (!_calledGet) {
                _calledGet = true;
                GC.SuppressFinalize(this);
            }

            _exception.Throw();
        }

        public AnabasisTaskStatus GetStatus(short token) => AnabasisTaskStatus.Faulted;

        public AnabasisTaskStatus UnsafeGetStatus() => AnabasisTaskStatus.Faulted;

        public void OnCompleted(Action<object?> continuation, object? state, short token) {
            continuation(state);
        }

        ~ExceptionResultSource() {
            if (!_calledGet) {
                AnabasisTaskScheduler.PublishUnobservedTaskException(_exception.SourceException);
            }
        }
    }

    private sealed class CanceledResultSource : IAnabasisTaskSource
    {
        private readonly CancellationToken _cancellationToken;

        public CanceledResultSource(CancellationToken cancellationToken) {
            _cancellationToken = cancellationToken;
        }

        public void GetResult(short token) {
            throw new OperationCanceledException(_cancellationToken);
        }

        public AnabasisTaskStatus GetStatus(short token) => AnabasisTaskStatus.Canceled;

        public AnabasisTaskStatus UnsafeGetStatus() => AnabasisTaskStatus.Canceled;

        public void OnCompleted(Action<object?> continuation, object? state, short token) {
            continuation(state);
        }
    }

    private sealed class CanceledResultSource<T> : IAnabasisTaskSource<T>
    {
        private readonly CancellationToken _cancellationToken;

        public CanceledResultSource(CancellationToken cancellationToken) {
            _cancellationToken = cancellationToken;
        }

        public T GetResult(short token) => throw new OperationCanceledException(_cancellationToken);

        void IAnabasisTaskSource.GetResult(short token) {
            throw new OperationCanceledException(_cancellationToken);
        }

        public AnabasisTaskStatus GetStatus(short token) => AnabasisTaskStatus.Canceled;

        public AnabasisTaskStatus UnsafeGetStatus() => AnabasisTaskStatus.Canceled;

        public void OnCompleted(Action<object?> continuation, object? state, short token) {
            continuation(state);
        }
    }

    private sealed class DeferPromise : IAnabasisTaskSource
    {
        private Func<AnabasisTask>? _factory;
        private AnabasisTask        _task;
        private Awaiter             _awaiter;

        public DeferPromise(Func<AnabasisTask> factory) {
            _factory = factory;
        }

        public void GetResult(short token) {
            _awaiter.GetResult();
        }

        public AnabasisTaskStatus GetStatus(short token) {
            if (Interlocked.Exchange(ref _factory, null) is { } f1) {
                _task = f1();
                _awaiter = _task.GetAwaiter();
            }

            return _task.Status;
        }

        public void OnCompleted(Action<object?> continuation, object? state, short token) {
            _awaiter.SourceOnCompleted(continuation, state);
        }

        public AnabasisTaskStatus UnsafeGetStatus() => _task.Status;
    }

    private sealed class DeferPromise<T> : IAnabasisTaskSource<T>
    {
        private Func<AnabasisTask<T>>?  _factory;
        private AnabasisTask<T>         _task;
        private AnabasisTask<T>.Awaiter _awaiter;

        public DeferPromise(Func<AnabasisTask<T>> factory) {
            _factory = factory;
        }

        public T GetResult(short token) => _awaiter.GetResult();

        void IAnabasisTaskSource.GetResult(short token) {
            _awaiter.GetResult();
        }

        public AnabasisTaskStatus GetStatus(short token) {
            if (Interlocked.Exchange(ref _factory, null) is { } f) {
                _task = f();
                _awaiter = _task.GetAwaiter();
            }

            return _task.Status;
        }

        public void OnCompleted(Action<object?> continuation, object? state, short token) {
            _awaiter.SourceOnCompleted(continuation, state);
        }

        public AnabasisTaskStatus UnsafeGetStatus() => _task.Status;
    }

    private sealed class NeverPromise<T> : IAnabasisTaskSource<T>
    {
        private readonly CancellationToken                   _cancellationToken;
        private          ManualResetAnabasisTaskSourceCore<T> _core;

        public NeverPromise(CancellationToken cancellationToken) {
            _cancellationToken = cancellationToken;
            if (_cancellationToken.CanBeCanceled) {
                _cancellationToken.RegisterWithoutCaptureExecutionContext(CancellationCallback, this);
            }
        }

        private static void CancellationCallback(object? state) {
            NeverPromise<T> self = (NeverPromise<T>)state!;
            self._core.TrySetCanceled(self._cancellationToken);
        }

        public T GetResult(short token) => _core.GetResult(token);

        public AnabasisTaskStatus GetStatus(short token) => _core.GetStatus(token);

        public AnabasisTaskStatus UnsafeGetStatus() => _core.UnsafeGetStatus();

        public void OnCompleted(Action<object?> continuation, object? state, short token) {
            _core.OnCompleted(continuation, state, token);
        }

        void IAnabasisTaskSource.GetResult(short token) {
            _core.GetResult(token);
        }
    }
}