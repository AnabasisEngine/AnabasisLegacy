using Silk.NET.OpenGL;

namespace Anabasis.Core.Handles;

public readonly record struct VertexArrayHandle(uint Value) : IAnabasisHandle
{
    public static ObjectIdentifier ObjectType => ObjectIdentifier.VertexArray;
    public void Free(GL gl) {
        gl.DeleteVertexArray(Value);
    }
}