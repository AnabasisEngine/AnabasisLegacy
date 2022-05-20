using Anabasis.Graphics.Abstractions.Textures;

namespace Anabasis.Images.Abstractions;

public interface IImageDataLoader
{
    public ValueTask<IImageDataSource> LoadAsync(Stream stream, CancellationToken cancellationToken = default);

    public ValueTask<ITexture2D> LoadAsync(ITextureSupport textureSupport,
        Stream stream, CancellationToken cancellationToken = default);
}