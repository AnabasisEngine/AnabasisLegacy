namespace Anabasis.Tasks.CompilerServices;

internal interface IAsyncStateMachineBox
{
    void MoveNext();

    Action MoveNextAction { get; }

    void Return();
}