using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace Anabasis.Tasks;

public class AnabasisTaskCompletionSource<T> : IAnabasisTaskSource<T>
{
    private CancellationToken                 _cancellationToken;
    private T?                                _result;
    private ExceptionHolder?                  _exception;
    private object?                           _gate;
    private Action<object?>?                  _singleContinuation;
    private object?                           _singleState;
    private List<(Action<object?>, object?)>? _secondaryContinuationList;

    private int  _intStatus; // AnabasisTaskStatus
    private bool _handled;

    [DebuggerHidden]
    internal void MarkHandled() {
        if (!_handled) {
            _handled = true;
        }
    }

    public AnabasisTask<T> Task {
        [DebuggerHidden] get => new(this, 0);
    }

    [DebuggerHidden]
    public bool TrySetResult(T result) {
        if (UnsafeGetStatus() != AnabasisTaskStatus.Pending) return false;

        _result = result;
        return TrySignalCompletion(AnabasisTaskStatus.Succeeded);
    }

    [DebuggerHidden]
    public bool TrySetCanceled(CancellationToken cancellationToken = default) {
        if (UnsafeGetStatus() != AnabasisTaskStatus.Pending) return false;

        _cancellationToken = cancellationToken;
        return TrySignalCompletion(AnabasisTaskStatus.Canceled);
    }

    [DebuggerHidden]
    public bool TrySetException(Exception exception) {
        if (exception is OperationCanceledException oce) {
            return TrySetCanceled(oce.CancellationToken);
        }

        if (UnsafeGetStatus() != AnabasisTaskStatus.Pending) return false;

        _exception = new ExceptionHolder(ExceptionDispatchInfo.Capture(exception));
        return TrySignalCompletion(AnabasisTaskStatus.Faulted);
    }

    [DebuggerHidden]
    public T GetResult(short token) {
        MarkHandled();

        AnabasisTaskStatus status = (AnabasisTaskStatus)_intStatus;
        switch (status) {
            case AnabasisTaskStatus.Succeeded:
                return _result!;
            case AnabasisTaskStatus.Faulted:
                _exception?.GetException().Throw();
                return default!;
            case AnabasisTaskStatus.Canceled:
                throw new OperationCanceledException(_cancellationToken);
            default:
            case AnabasisTaskStatus.Pending:
                throw new InvalidOperationException("not yet completed.");
        }
    }

    [DebuggerHidden]
    void IAnabasisTaskSource.GetResult(short token) {
        GetResult(token);
    }

    [DebuggerHidden]
    public AnabasisTaskStatus GetStatus(short token) => (AnabasisTaskStatus)_intStatus;

    [DebuggerHidden]
    public AnabasisTaskStatus UnsafeGetStatus() => (AnabasisTaskStatus)_intStatus;

    [DebuggerHidden]
    public void OnCompleted(Action<object?> continuation, object? state, short token) {
        if (_gate == null) {
            Interlocked.CompareExchange(ref _gate, new object(), null);
        }

        object lockGate = Thread.VolatileRead(ref _gate);
        lock (lockGate) // wait TrySignalCompletion, after status is not pending.
        {
            if ((AnabasisTaskStatus)_intStatus != AnabasisTaskStatus.Pending) {
                continuation(state);
                return;
            }

            if (_singleContinuation == null) {
                _singleContinuation = continuation;
                _singleState = state;
            } else {
                if (_secondaryContinuationList == null) {
                    _secondaryContinuationList = new List<(Action<object?>, object?)>();
                }

                _secondaryContinuationList.Add((continuation, state));
            }
        }
    }

    [DebuggerHidden]
    private bool TrySignalCompletion(AnabasisTaskStatus status) {
        if (Interlocked.CompareExchange(ref _intStatus, (int)status, (int)AnabasisTaskStatus.Pending) !=
            (int)AnabasisTaskStatus.Pending) return false;
        if (_gate == null) {
            Interlocked.CompareExchange(ref _gate, new object(), null);
        }

        object lockGate = Thread.VolatileRead(ref _gate);
        lock (lockGate) // wait OnCompleted.
        {
            if (_singleContinuation != null) {
                try {
                    _singleContinuation(_singleState);
                }
                catch (Exception ex) {
                    AnabasisTaskScheduler.PublishUnobservedTaskException(ex);
                }
            }

            if (_secondaryContinuationList != null) {
                foreach ((Action<object?> c, object? state) in _secondaryContinuationList) {
                    try {
                        c(state);
                    }
                    catch (Exception ex) {
                        AnabasisTaskScheduler.PublishUnobservedTaskException(ex);
                    }
                }
            }

            _singleContinuation = null;
            _singleState = null;
            _secondaryContinuationList = null;
        }

        return true;

    }
}