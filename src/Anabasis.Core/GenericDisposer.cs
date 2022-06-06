namespace Anabasis.Core;

public sealed class GenericDisposer : IDisposable
{
    private readonly Action _disposeAction;

    public GenericDisposer(Action disposeAction) {
        _disposeAction = disposeAction;
    }

    public void Dispose() {
        _disposeAction();
    }
}