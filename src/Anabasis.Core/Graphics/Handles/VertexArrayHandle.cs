using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Handles;

public readonly record struct VertexArrayHandle(uint Value) : IAnabasisHandle<GL>, IAnabasisBindableHandle<GL>, IKindedHandle
{
    public static ObjectIdentifier ObjectType => ObjectIdentifier.VertexArray;
    public void Free(GL api) {
        api.DeleteVertexArray(Value);
    }

    public void Use(GL api) {
        api.BindVertexArray(Value);
    }
}