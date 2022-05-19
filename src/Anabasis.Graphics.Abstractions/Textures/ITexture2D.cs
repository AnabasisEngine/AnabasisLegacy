namespace Anabasis.Graphics.Abstractions.Textures;

public interface ITexture2D : ITexture
{
    int Width { get; }
    int Height { get; }
    void UploadPixels(int level, Range xRange, Range yRange, Span<Color> pixels);
}