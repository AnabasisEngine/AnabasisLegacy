namespace Anabasis.Graphics.Abstractions.Textures;

public interface ISupportRawPixelUpload2D<in TPixelFormat, in TPixelType> : ITexture2D
    where TPixelFormat : Enum
    where TPixelType : Enum
{
    public void UploadPixels<TPixel>(int level, Range xRange, Range yRange, TPixelFormat format, TPixelType type,
        ReadOnlySpan<TPixel> pixels)
        where TPixel : unmanaged;
}