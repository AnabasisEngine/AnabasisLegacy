using Anabasis.Platform.Abstractions;
using Anabasis.Platform.Abstractions.Resources;

namespace Anabasis.Platform.Silk.Internal;

public abstract class SilkGlObject<TName> : IGlObject<TName>, IPlatformResource
    where TName : struct, IGlHandle, IPlatformHandle
{
    private string? _label;

    protected SilkGlObject(IGlApi gl, TName name) {
        Gl = gl;
        Handle = name;
    }

    protected IGlApi Gl { get; }

    public TName Handle { get; }

    IGlHandle IGlObject.Handle => Handle;

    IPlatformHandle IPlatformResource.Handle => Handle;

    public string Label {
        get => _label ??= Gl.GetObjectLabel(Handle);
        set {
            _label = value;
            Gl.ObjectLabel(Handle, value);
        }
    }

    public abstract IDisposable Use();
    public abstract void Dispose();
}