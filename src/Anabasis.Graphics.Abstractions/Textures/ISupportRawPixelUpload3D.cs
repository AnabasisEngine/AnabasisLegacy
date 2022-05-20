namespace Anabasis.Graphics.Abstractions.Textures;

public interface ISupportRawPixelUpload3D<in TPixelFormat, in TPixelType>: ITexture3D
    where TPixelFormat : Enum
    where TPixelType : Enum
{
    public void UploadPixels<TPixel>(int level, Range xRange, Range yRange, Range zRange, TPixelFormat format,
        TPixelType type,
        Span<TPixel> pixels)
        where TPixel : unmanaged;
}