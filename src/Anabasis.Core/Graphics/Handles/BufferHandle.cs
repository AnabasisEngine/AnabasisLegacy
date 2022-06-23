using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Handles;

public readonly record struct BufferHandle(uint Value) : IAnabasisHandle<GL>, IKindedHandle
{
    public static ObjectIdentifier ObjectType => ObjectIdentifier.Buffer;
    public void Free(GL api) {
        api.DeleteBuffer(Value);
    }
}