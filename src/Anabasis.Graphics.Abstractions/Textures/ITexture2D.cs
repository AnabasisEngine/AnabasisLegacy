namespace Anabasis.Graphics.Abstractions.Textures;

public interface ITextureView2D
{
    void UploadPixels(int level, Range xRange, Range yRange, ReadOnlySpan<Color> pixels);
    int Width { get; }
    int Height { get; }
}

public interface ITexture2D : ITexture, ITextureView2D
{ }