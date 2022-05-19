using Anabasis.Platform.Abstractions.Resources;

namespace Anabasis.Graphics.Abstractions.Textures;

public interface ITexture : IPlatformResource
{
    ITextureBinding Bind(int unit);
    int MipmapLevels { get; }
    void GenerateMipmap();
}