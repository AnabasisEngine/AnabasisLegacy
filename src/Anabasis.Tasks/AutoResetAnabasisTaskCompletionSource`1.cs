using System.Diagnostics;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Anabasis.Tasks;

public class AutoResetAnabasisTaskCompletionSource<T> : IAnabasisTaskSource<T>
{
    private static readonly ObjectPool<AutoResetAnabasisTaskCompletionSource<T>> Pool = new(() =>
        new AutoResetAnabasisTaskCompletionSource<T>());

    private ManualResetAnabasisTaskSourceCore<T> _core;

    private AutoResetAnabasisTaskCompletionSource() { }

    [DebuggerHidden]
    public static AutoResetAnabasisTaskCompletionSource<T> Create() => Pool.Allocate();

    [DebuggerHidden]
    public static AutoResetAnabasisTaskCompletionSource<T> CreateFromCanceled(CancellationToken cancellationToken,
        out short token) {
        AutoResetAnabasisTaskCompletionSource<T> source = Create();
        source.TrySetCanceled(cancellationToken);
        token = source._core.Version;
        return source;
    }

    [DebuggerHidden]
    public static AutoResetAnabasisTaskCompletionSource<T> CreateFromException(Exception exception, out short token) {
        AutoResetAnabasisTaskCompletionSource<T> source = Create();
        source.TrySetException(exception);
        token = source._core.Version;
        return source;
    }

    [DebuggerHidden]
    public static AutoResetAnabasisTaskCompletionSource<T> CreateFromResult(T result, out short token) {
        AutoResetAnabasisTaskCompletionSource<T> source = Create();
        source.TrySetResult(result);
        token = source._core.Version;
        return source;
    }

    public AnabasisTask<T> Task {
        [DebuggerHidden] get => new(this, _core.Version);
    }

    [DebuggerHidden]
    public bool TrySetResult(T result) => _core.TrySetResult(result);

    [DebuggerHidden]
    public bool TrySetCanceled(CancellationToken cancellationToken = default) =>
        _core.TrySetCanceled(cancellationToken);

    [DebuggerHidden]
    public bool TrySetException(Exception exception) => _core.TrySetException(exception);

    [DebuggerHidden]
    public T GetResult(short token) {
        try {
            return _core.GetResult(token);
        }
        finally {
            TryReturn();
        }
    }

    [DebuggerHidden]
    void IAnabasisTaskSource.GetResult(short token) {
        GetResult(token);
    }

    [DebuggerHidden]
    public AnabasisTaskStatus GetStatus(short token) => _core.GetStatus(token);

    [DebuggerHidden]
    public AnabasisTaskStatus UnsafeGetStatus() => _core.UnsafeGetStatus();

    [DebuggerHidden]
    public void OnCompleted(Action<object?> continuation, object? state, short token) {
        _core.OnCompleted(continuation, state, token);
    }

    [DebuggerHidden]
    private void TryReturn() {
        _core.Reset();
        Pool.Free(this);
    }
}