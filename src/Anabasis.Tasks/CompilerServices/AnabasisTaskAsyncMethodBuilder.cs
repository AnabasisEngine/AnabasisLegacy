using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Anabasis.Tasks.CompilerServices;

[StructLayout(LayoutKind.Auto)]
public struct AnabasisTaskAsyncMethodBuilder
{
    private IStateMachineRunnerPromise? _runnerPromise;
    private Exception?                  _ex;

    // 1. Static Create method.
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AnabasisTaskAsyncMethodBuilder Create() => default;

    public AnabasisTask Task {
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _runnerPromise?.Task ?? (_ex != null ? AnabasisTask.FromException(_ex) : AnabasisTask.CompletedTask);
    }

    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetException(Exception exception) {
        if (_runnerPromise == null) {
            _ex = exception;
        } else {
            _runnerPromise.SetException(exception);
        }
    }

    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetResult() {
        _runnerPromise?.SetResult();
    }

    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine {
        if (_runnerPromise == null) {
            AsyncAnabasisTask<TStateMachine>.SetStateMachine(ref stateMachine, out _runnerPromise);
        }

        awaiter.OnCompleted(_runnerPromise.MoveNext);
    }

    [DebuggerHidden]
    [SecuritySafeCritical]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine {
        if (_runnerPromise == null) {
            AsyncAnabasisTask<TStateMachine>.SetStateMachine(ref stateMachine, out _runnerPromise);
        }

        awaiter.UnsafeOnCompleted(_runnerPromise.MoveNext);
    }
    
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
    {
        stateMachine.MoveNext();
    }
    
    [DebuggerHidden]
    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        // don't use boxed stateMachine.
    }
}