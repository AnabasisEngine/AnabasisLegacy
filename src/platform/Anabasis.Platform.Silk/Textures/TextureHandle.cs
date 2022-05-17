using Anabasis.Platform.Abstractions;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Textures;

public readonly record struct TextureHandle(uint Value) : IPlatformHandle, IGlHandle
{
    public static ObjectIdentifier ObjectType => ObjectIdentifier.Texture;
}