using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics;

public abstract class AnabasisGraphicsObject<THandle> : AnabasisNativeObject<GL, THandle>, ILabellableObject
    where THandle : struct, IAnabasisHandle<GL>, IKindedHandle
{
    private string? _label;
    protected AnabasisGraphicsObject(GL api, THandle name) : base(api, name) { }

    public string Label {
        get => _label ??= Api.GetObjectLabel(THandle.ObjectType, Handle.Value);
        set {
            _label = value;
            Api.ObjectLabel(THandle.ObjectType, Handle.Value, value);
        }
    }
}