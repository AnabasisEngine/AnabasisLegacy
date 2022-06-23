using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Handles;

public readonly record struct TextureHandle(uint Value) : IAnabasisHandle<GL>, IKindedHandle
{
    public static ObjectIdentifier ObjectType => ObjectIdentifier.Texture;
    public void Free(GL api) {
        api.DeleteTexture(Value);
    }
}