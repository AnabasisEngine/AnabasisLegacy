using Silk.NET.OpenGL;

namespace Anabasis.Core.Handles;

public readonly record struct BufferHandle(uint Value) : IAnabasisHandle
{
    public static ObjectIdentifier ObjectType => ObjectIdentifier.Buffer;
    public void Free(GL gl) {
        gl.DeleteBuffer(Value);
    }
}