using System.Diagnostics;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Anabasis.Tasks;

public class AutoResetAnabasisTaskCompletionSource : IAnabasisTaskSource
{
    private static readonly ObjectPool<AutoResetAnabasisTaskCompletionSource> Pool = new(() =>
        new AutoResetAnabasisTaskCompletionSource());

    private ManualResetAnabasisTaskSourceCore<AsyncUnit> _core;

    private AutoResetAnabasisTaskCompletionSource() { }

    [DebuggerHidden]
    public static AutoResetAnabasisTaskCompletionSource Create() => Pool.Allocate();

    [DebuggerHidden]
    public static AutoResetAnabasisTaskCompletionSource CreateFromCanceled(CancellationToken cancellationToken,
        out short token) {
        AutoResetAnabasisTaskCompletionSource source = Create();
        source.TrySetCanceled(cancellationToken);
        token = source._core.Version;
        return source;
    }

    [DebuggerHidden]
    public static AutoResetAnabasisTaskCompletionSource CreateFromException(Exception exception, out short token) {
        AutoResetAnabasisTaskCompletionSource source = Create();
        source.TrySetException(exception);
        token = source._core.Version;
        return source;
    }

    [DebuggerHidden]
    public static AutoResetAnabasisTaskCompletionSource CreateCompleted(out short token) {
        AutoResetAnabasisTaskCompletionSource source = Create();
        source.TrySetResult();
        token = source._core.Version;
        return source;
    }

    public AnabasisTask Task {
        [DebuggerHidden] get => new(this, _core.Version);
    }

    [DebuggerHidden]
    public bool TrySetResult() => _core.TrySetResult(AsyncUnit.Default);

    [DebuggerHidden]
    public bool TrySetCanceled(CancellationToken cancellationToken = default) =>
        _core.TrySetCanceled(cancellationToken);

    [DebuggerHidden]
    public bool TrySetException(Exception exception) => _core.TrySetException(exception);

    [DebuggerHidden]
    public void GetResult(short token) {
        try {
            _core.GetResult(token);
        }
        finally {
            TryReturn();
        }
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