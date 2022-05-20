using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Anabasis.Threading.CompilerServices;

internal sealed class AnabasisVoidRunner<TStateMachine> : IAsyncStateMachineBox
    where TStateMachine : IAsyncStateMachine
{
    internal TStateMachine StateMachine = default!;

    private static readonly ObjectPool<AnabasisVoidRunner<TStateMachine>> Pool = new(() =>
        new AnabasisVoidRunner<TStateMachine>());

    public static AnabasisVoidRunner<TStateMachine> Get() => Pool.Allocate();

    public void Return() => Pool.Free(this);

    private AnabasisVoidRunner() {
        MoveNextAction = MoveNext;
    }

    public void MoveNext() {
        StateMachine.MoveNext();
    }

    public Action MoveNextAction { get; }
}