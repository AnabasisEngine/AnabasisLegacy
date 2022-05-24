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

    internal SilkTexture2D(IGlApi glApi, TextureHandle handle) : base(glApi, handle, TextureTarget.Texture2D) { }

    public int Width { get; internal init; }
    public int Height { get; internal init; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UploadPixels(int level, int xOffset, uint width, int yOffset, uint height,
        ReadOnlySpan<Color> pixels) =>
        UploadPixels(level, xOffset, width, yOffset, height, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

    public void UploadPixels<TPixel>(int level, int xOffset, uint width, int yOffset, uint height, PixelFormat format,
        PixelType type, ReadOnlySpan<TPixel> pixels)
        where TPixel : unmanaged {
        Gl.TextureSubImage2D(Handle, level, xOffset, yOffset, width, height, format, type,
            in pixels.GetPinnableReference());
    }
}