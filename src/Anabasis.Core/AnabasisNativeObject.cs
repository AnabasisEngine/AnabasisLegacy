namespace Anabasis.Core;

public abstract class AnabasisNativeObject<TApi, THandle>
    : IAnabasisNativeObject<TApi, THandle>
    where THandle : struct, IAnabasisHandle<TApi>
{
    protected AnabasisNativeObject(TApi api, THandle name) {
        Api = api;
        Handle = name;
    }

    protected internal TApi Api { get; }
    public THandle Handle { get; }

    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            Handle.Free(Api);
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}