namespace Anabasis.Graphics.Abstractions.Textures;

public interface ITextureSupport
{
    public ValueTask<ITexture2D>
        CreateTexture2DAsync(IGraphicsDevice graphicsDevice, int levels, int width, int height);

    public ValueTask<ITexture3D> CreateTexture3DAsync(IGraphicsDevice graphicsDevice, int levels, int width, int height,
        int depth);

    public ValueTask<ITexture2DArray> CreateTexture2DArrayAsync(IGraphicsDevice graphicsDevice, int levels, int width,
        int height, int layers);
}