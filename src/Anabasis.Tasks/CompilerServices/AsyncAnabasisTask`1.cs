using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Anabasis.Tasks.CompilerServices;

internal sealed class AsyncAnabasisTask<TStateMachine> : IStateMachineRunnerPromise, IAnabasisTaskSource
    where TStateMachine : IAsyncStateMachine
{
    private static readonly ObjectPool<AsyncAnabasisTask<TStateMachine>> Pool = new(() =>
        new AsyncAnabasisTask<TStateMachine>());

    public Action MoveNext { get; }

    private TStateMachine?                              _stateMachine;
    private AnabasisTaskCompletionSourceCore<AsyncUnit> _core;

    private AsyncAnabasisTask() {
        MoveNext = Run;
    }

    public static void SetStateMachine(ref TStateMachine stateMachine,
        out IStateMachineRunnerPromise runnerPromiseFieldRef) {
        AsyncAnabasisTask<TStateMachine> result = Pool.Allocate();

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

    public AnabasisTask Task {
        [DebuggerHidden] get => new AnabasisTask(this, _core.Version);
    }

    [DebuggerHidden]
    public void SetResult() {
        _core.TrySetResult(AsyncUnit.Default);
    }

    [DebuggerHidden]
    public void SetException(Exception exception) {
        _core.TrySetException(exception);
    }

    [DebuggerHidden]
    public void GetResult(short token) {
        try {
            _core.GetResult(token);
        }
        finally {
            Free();
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
}