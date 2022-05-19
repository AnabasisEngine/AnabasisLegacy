namespace Anabasis.Graphics.Abstractions.Textures;

public interface ITextureSupport
{
    public ITexture2D CreateTexture2D(IGraphicsDevice graphicsDevice, int levels, int width, int height);
    public ITexture3D CreateTexture3D(IGraphicsDevice graphicsDevice, int levels, int width, int height, int depth);
    public ITexture2DArray CreateTexture2DArray(IGraphicsDevice graphicsDevice, int levels, int width, int height, int layers);
}