using System.Runtime.CompilerServices;
using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Platform.Silk.Internal;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Textures;

public class SilkTexture2D : SilkTexture, ITexture2D, ISupportRawPixelUpload2D<PixelFormat, PixelType>
{
    public SilkTexture2D(IGlApi glApi, SizedInternalFormat format, int levels, int width, int height) : base(glApi,
        TextureTarget.Texture2D) {
        MipmapLevels = levels;
        Width = width;
        Height = height;
        Gl.TextureStorage2D(Handle, (uint)levels, format, (uint)width, (uint)height);
    }

    internal SilkTexture2D(IGlApi glApi, TextureHandle handle) : base(glApi, handle) { }

    public int Width { get; internal init; }
    public int Height { get; internal init; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UploadPixels(int level, Range xRange, Range yRange, Span<Color> pixels) =>
        UploadPixels(level, xRange, yRange, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

    public void UploadPixels<TPixel>(int level, Range xRange, Range yRange, PixelFormat format, PixelType type,
        Span<TPixel> pixels)
        where TPixel : unmanaged {
        (int xOffset, int width) = xRange.GetOffsetAndLength(Width);
        (int yOffset, int height) = yRange.GetOffsetAndLength(Height);
        Gl.TextureSubImage2D(Handle, level, xOffset, yOffset, (uint)width, (uint)height, format, type,
            in pixels.GetPinnableReference());
    }
}