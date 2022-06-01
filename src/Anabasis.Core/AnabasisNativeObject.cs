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
        get => _label ??= Gl.GetObjectLabel(THandle.ObjectType, Handle.Value);
        set {
            _label = value;
            Gl.ObjectLabel(THandle.ObjectType, Handle.Value, value);
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