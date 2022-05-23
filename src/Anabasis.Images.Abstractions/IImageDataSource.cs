using Anabasis.Graphics.Abstractions.Textures;

namespace Anabasis.Images.Abstractions;

public interface IImageDataSource
{
    public void UploadToTexture(ITextureView2D texture);
}