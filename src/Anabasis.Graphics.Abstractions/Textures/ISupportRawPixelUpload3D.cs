namespace Anabasis.Graphics.Abstractions.Textures;

public interface ISupportRawPixelUpload3D<in TPixelFormat, in TPixelType> : ITexture3D
    where TPixelFormat : Enum
    where TPixelType : Enum
{
    public void UploadPixels<TPixel>(int level, int xOffset, uint xRange, int yOffset, uint yRange, int zOffset,
        uint zRange, TPixelFormat format, TPixelType type, ReadOnlySpan<TPixel> pixels)
        where TPixel : unmanaged;
}