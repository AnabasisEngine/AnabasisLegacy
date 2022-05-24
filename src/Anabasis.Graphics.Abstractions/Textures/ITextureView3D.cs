using Silk.NET.Maths;

namespace Anabasis.Graphics.Abstractions.Textures;

public interface ITextureView3D
{
    int Width { get; }
    int Height { get; }
    int Depth { get; }

    public Vector3D<int> Size => new(Width, Height, Depth);

    void UploadPixels(int level, int xOffset, uint width, int yOffset, uint height, int zOffset, uint depth,
        ReadOnlySpan<Color> pixels);
}