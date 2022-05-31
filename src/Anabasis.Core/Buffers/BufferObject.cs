using Anabasis.Core.Handles;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Buffers;

public class BufferObject : AnabasisNativeObject<BufferHandle>
{
    public BufferObject(GL gl) : base(gl, new BufferHandle(gl.CreateBuffer())) {
    }

    public IDisposable Use(BufferTargetARB target) {
        Gl.BindBuffer(target, Handle.Value);
        return new GenericDisposer(() => Gl.BindBuffer(target, 0));
    }
}