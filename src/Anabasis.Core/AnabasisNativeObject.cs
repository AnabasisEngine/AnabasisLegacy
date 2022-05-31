using Silk.NET.OpenGL;

namespace Anabasis.Core;

public abstract class AnabasisNativeObject<THandle> : IAnabasisNativeObject<THandle>
    where THandle : struct, IAnabasisHandle
{
    private string? _label;
    protected AnabasisNativeObject(GL gl, THandle name) {
        Gl = gl;
        Handle = name;
    }
    protected GL Gl { get; }
    public THandle Handle { get; }
    public string Label {
        get => _label ??= Gl.GetObjectLabel((ObjectIdentifier)THandle.ObjectType, Handle.Value);
        set {
            _label = value;
            Gl.ObjectLabel((ObjectIdentifier)THandle.ObjectType, Handle.Value, value);
        }
    }

    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            Handle.Free(Gl);
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

public abstract class AnabasisBindableNativeObject<THandle> : AnabasisNativeObject<THandle>, IAnabasisBindableObject
    where THandle : struct, IAnabasisBindableHandle
{
    protected AnabasisBindableNativeObject(GL gl, THandle name) : base(gl, name) { }
    public IDisposable Use() {
        Handle.Use(Gl);
        return new GenericDisposer(() => default(THandle).Use(Gl));
    }
}