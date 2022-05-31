using Silk.NET.OpenGL;

namespace Anabasis.Core.Handles;

public readonly record struct VertexArrayHandle(uint Value) : IAnabasisHandle, IAnabasisBindableHandle
{
    public static ObjectIdentifier ObjectType => ObjectIdentifier.VertexArray;
    public void Free(GL gl) {
        gl.DeleteVertexArray(Value);
    }

    public void Use(GL gl) {
        gl.BindVertexArray(Value);
    }
}