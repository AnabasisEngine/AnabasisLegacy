namespace Anabasis.Tasks.CompilerServices;

internal interface IStateMachineRunnerPromise : IAnabasisTaskSource
{
    Action MoveNext { get; }
    AnabasisTask Task { get; }
    void SetResult();
    void SetException(Exception exception);
}