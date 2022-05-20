namespace Anabasis.Graphics.Abstractions.Textures;

public interface ITexture3D : ITexture
{
    int Width { get; }
    int Height { get; }
    int Depth { get; }
    void UploadPixels(int level, Range xRange, Range yRange, Range zRange, ReadOnlySpan<Color> pixels);
}