namespace Anabasis.Tasks.CompilerServices;

internal interface IStateMachineRunnerPromise<T> : IAnabasisTaskSource<T>
{
    Action MoveNext { get; }
    AnabasisTask<T> Task { get; }
    void SetResult(T result);
    void SetException(Exception exception);
}