namespace Anabasis.Threading.CompilerServices;

internal interface IAsyncStateMachineBox
{
    void MoveNext();

    Action MoveNextAction { get; }

    void Return();
}