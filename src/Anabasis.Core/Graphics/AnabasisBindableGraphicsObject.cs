using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics;

public abstract class AnabasisBindableGraphicsObject<THandle>
    : AnabasisGraphicsObject<THandle>, IAnabasisBindableObject<GL>
    where THandle : struct, IAnabasisBindableHandle<GL>, IKindedHandle
{
    protected AnabasisBindableGraphicsObject(GL api, THandle name) : base(api, name) { }

    public IDisposable Use() {
        Handle.Use(Api);
        return new GenericDisposer(() => default(THandle).Use(Api));
    }
}