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