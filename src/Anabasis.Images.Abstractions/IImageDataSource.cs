using Anabasis.Graphics.Abstractions.Textures;

namespace Anabasis.Images.Abstractions;

public interface IImageDataSource
{
    public void UploadToTexture(ITexture2D texture);
}