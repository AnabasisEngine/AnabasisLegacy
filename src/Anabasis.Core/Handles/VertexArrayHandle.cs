using Silk.NET.OpenGL;

namespace Anabasis.Core.Handles;

public readonly record struct VertexArrayHandle(uint Value) : IAnabasisHandle
{
    public static ObjectType ObjectType => ObjectType.VertexArray;
    public void Free(GL gl) {
        gl.DeleteVertexArray(Value);
    }
}