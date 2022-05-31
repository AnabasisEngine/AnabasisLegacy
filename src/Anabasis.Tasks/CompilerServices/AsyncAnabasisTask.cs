using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Anabasis.Tasks.CompilerServices;

internal sealed class AsyncAnabasisTask<TStateMachine, T> : IStateMachineRunnerPromise<T>, IAnabasisTaskSource<T>
    where TStateMachine : IAsyncStateMachine
{
    static readonly ObjectPool<AsyncAnabasisTask<TStateMachine, T>> Pool = new(() => new AsyncAnabasisTask<TStateMachine, T>());

    public Action MoveNext { get; }

    private TStateMachine?                      _stateMachine;
    private AnabasisTaskCompletionSourceCore<T> _core;

    private AsyncAnabasisTask() {
        MoveNext = Run;
    }

    public static void SetStateMachine(ref TStateMachine stateMachine,
        out IStateMachineRunnerPromise<T> runnerPromiseFieldRef) {
        AsyncAnabasisTask<TStateMachine, T>? result = Pool.Allocate();

        runnerPromiseFieldRef = result; // set runner before copied.
        result._stateMachine = stateMachine; // copy struct StateMachine(in release build).
    }

    void Free() {
        _core.Reset();
        _stateMachine = default;
        Pool.Free(this);
    }

    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Run() {
        _stateMachine?.MoveNext();
    }

    public AnabasisTask<T> Task {
        [DebuggerHidden] get => new AnabasisTask<T>(this, _core.Version);
    }

    [DebuggerHidden]
    public void SetResult(T result) {
        _core.TrySetResult(result);
    }

    [DebuggerHidden]
    public void SetException(Exception exception) {
        _core.TrySetException(exception);
    }

    [DebuggerHidden]
    public T GetResult(short token) {
        try {
            return _core.GetResult(token);
        }
        finally {
            Free();
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
}