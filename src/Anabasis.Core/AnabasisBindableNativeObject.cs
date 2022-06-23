namespace Anabasis.Core;

public abstract class AnabasisBindableNativeObject<TApi, THandle>
    : AnabasisNativeObject<TApi, THandle>, IAnabasisBindableObject<TApi>
    where THandle : struct, IAnabasisBindableHandle<TApi>
{
    protected AnabasisBindableNativeObject(TApi api, THandle name) : base(api, name) { }

    public IDisposable Use() {
        Handle.Use(Api);
        return new GenericDisposer(() => default(THandle).Use(Api));
    }
}