namespace Anabasis.Graphics.Abstractions.Textures;

public interface ITextureView3D
{
    int Width { get; }
    int Height { get; }
    int Depth { get; }

    void UploadPixels(int level, int xOffset, uint width, int yOffset, uint height, int zOffset, uint depth,
        ReadOnlySpan<Color> pixels);
}