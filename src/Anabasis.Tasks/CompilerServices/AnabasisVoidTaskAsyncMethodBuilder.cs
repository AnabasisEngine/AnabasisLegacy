using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Anabasis.Tasks.CompilerServices;

public struct AnabasisVoidTaskAsyncMethodBuilder
{
    private IAsyncStateMachineBox? _box;
    public static AnabasisVoidTaskAsyncMethodBuilder Create() => default;

    public void SetResult() {
        _box?.Return();
    }

    [DebuggerStepThrough]
    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine {
        stateMachine.MoveNext();
    }

    public AnabasisVoidTask Task => default;

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine {
        _box ??= CreateBox(ref stateMachine);
        awaiter.OnCompleted(_box.MoveNextAction);
    }

    private static IAsyncStateMachineBox CreateBox<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine {
        AnabasisVoidRunner<TStateMachine> box = AnabasisVoidRunner<TStateMachine>.Get();
        box.StateMachine = stateMachine;
        return box;
    }

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine {
        _box ??= CreateBox(ref stateMachine);
        awaiter.OnCompleted(_box.MoveNextAction);
    }

    [DebuggerHidden]
    public void SetException(Exception e) {
        // AnabasisTaskManager.Current.ScheduleContinuation(() => ExceptionDispatchInfo.Throw(e));
        _box?.Return();
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine) { }
}