namespace Anabasis.Core;

public interface ILabellableObject
{

    public string Label { get; set; }
}

public interface IAnabasisNativeObject<TApi> : IDisposable
{
    public IAnabasisHandle<TApi> Handle { get; }
}

public interface IAnabasisNativeObject<TApi, out TName> : IAnabasisNativeObject<TApi>
    where TName : struct, IAnabasisHandle<TApi>
{
    public new TName Handle { get; }
    IAnabasisHandle<TApi> IAnabasisNativeObject<TApi>.Handle => Handle;
}