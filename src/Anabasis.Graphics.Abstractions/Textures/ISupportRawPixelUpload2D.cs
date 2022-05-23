namespace Anabasis.Graphics.Abstractions.Textures;

public interface ISupportRawPixelUpload2D<in TPixelFormat, in TPixelType> : ITextureView2D
    where TPixelFormat : Enum
    where TPixelType : Enum
{
    public void UploadPixels<TPixel>(int level, int xOffset, uint xSize, int yOffset, uint ySize, TPixelFormat format,
        TPixelType type, ReadOnlySpan<TPixel> pixels)
        where TPixel : unmanaged;
}

public interface ISupportCopyTextureData
{
    public void CopyPixels(ITexture source, ITexture dest, int srcX, int srcY, int srcZ, int srcLevel, int destX,
        int destY, int destZ, int destLevel, uint srcWidth, uint srcHeight, uint srcDepth);
}