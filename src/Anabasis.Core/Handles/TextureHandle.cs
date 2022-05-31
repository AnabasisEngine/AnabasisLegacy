using Silk.NET.OpenGL;

namespace Anabasis.Core.Handles;

public readonly record struct TextureHandle(uint Value) : IAnabasisHandle
{
    public static ObjectType ObjectType => ObjectType.Texture;
    public void Free(GL gl) {
        gl.DeleteTexture(Value);
    }
}