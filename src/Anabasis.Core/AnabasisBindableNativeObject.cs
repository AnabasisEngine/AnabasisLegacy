using Silk.NET.OpenGL;

namespace Anabasis.Core;

public abstract class AnabasisBindableNativeObject<THandle> : AnabasisNativeObject<THandle>, IAnabasisBindableObject
    where THandle : struct, IAnabasisBindableHandle
{
    protected AnabasisBindableNativeObject(GL gl, THandle name) : base(gl, name) { }
    public IDisposable Use() {
        Handle.Use(Gl);
        return new GenericDisposer(() => default(THandle).Use(Gl));
    }
}