using Silk.NET.OpenGL;

namespace Anabasis.Core.Handles;

public readonly record struct BufferHandle(uint Value) : IAnabasisHandle
{
    public static ObjectType ObjectType => ObjectType.Buffer;
    public void Free(GL gl) {
        gl.DeleteBuffer(Value);
    }
}