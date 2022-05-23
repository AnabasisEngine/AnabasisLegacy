namespace Anabasis.Graphics.Abstractions.Textures;

public interface ITextureView2D
{
    void UploadPixels(int level, int xOffset, uint xRange, int yOffset, uint yRange, ReadOnlySpan<Color> pixels);
    int Width { get; }
    int Height { get; }
}