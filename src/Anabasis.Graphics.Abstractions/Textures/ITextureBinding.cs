using Anabasis.Platform.Abstractions;

namespace Anabasis.Graphics.Abstractions.Textures;

public interface ITextureBinding
{
    IPlatformHandle TextureHandle { get; }
}