namespace Anabasis.Core;

public interface IAnabasisNativeObject : IDisposable
{
    public IAnabasisHandle Handle { get; }

    public string Label { get; set; }
}

public interface IAnabasisNativeObject<out TName> : IAnabasisNativeObject
    where TName : struct, IAnabasisHandle
{
    public new TName Handle { get; }
    IAnabasisHandle IAnabasisNativeObject.Handle => Handle;
}

public interface IAnabasisBindableObject : IAnabasisNativeObject
{
    public IDisposable Use();
}

public class GenericDisposer : IDisposable
{
    private readonly Action _disposeAction;

    public GenericDisposer(Action disposeAction) {
        _disposeAction = disposeAction;
    }

    public void Dispose() {
        _disposeAction();
    }
}