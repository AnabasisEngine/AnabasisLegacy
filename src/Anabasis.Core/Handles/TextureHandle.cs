using Silk.NET.OpenGL;

namespace Anabasis.Core.Handles;

public readonly record struct TextureHandle(uint Value) : IAnabasisHandle
{
    public static ObjectIdentifier ObjectType => ObjectIdentifier.Texture;
    public void Free(GL gl) {
        gl.DeleteTexture(Value);
    }
}