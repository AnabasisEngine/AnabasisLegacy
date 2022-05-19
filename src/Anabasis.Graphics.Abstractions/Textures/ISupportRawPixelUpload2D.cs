namespace Anabasis.Graphics.Abstractions.Textures;

public interface ISupportRawPixelUpload2D<in TPixelFormat, in TPixelType>
    where TPixelFormat : Enum
    where TPixelType : Enum
{
    public void UploadPixels<TPixel>(int level, Range xRange, Range yRange, TPixelFormat format, TPixelType type,
        Span<TPixel> pixels)
        where TPixel : unmanaged;
}