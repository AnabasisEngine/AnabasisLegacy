using System.Runtime.CompilerServices;
using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Platform.Silk.Internal;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Textures;

public class SilkTexture3D
    : SilkTexture, ITexture3D, ISupportRawPixelUpload3D<PixelFormat, PixelType>
{
    public SilkTexture3D(IGlApi glApi, SizedInternalFormat format, int levels, int width, int height, int depth) : this
        (glApi, TextureTarget.Texture3D, format, levels, width, height, depth) { }

    protected SilkTexture3D(IGlApi glApi, TextureTarget target, SizedInternalFormat format, int levels, int width,
        int height, int depth) : base(glApi, target) {
        MipmapLevels = levels;
        Width = width;
        Height = height;
        Depth = depth;
        Format = format;
        Gl.TextureStorage3D(Handle, (uint)levels, format, (uint)width, (uint)height, (uint)depth);
    }

    protected SilkTexture3D(IGlApi glApi, TextureHandle handle) : base(glApi, handle) { }
    public int Width { get; protected init; }
    public int Height { get; protected init; }
    public int Depth { get; protected init; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UploadPixels(int level, Range xRange, Range yRange, Range zRange, Span<Color> pixels) =>
        UploadPixels(level, xRange, yRange, zRange, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

    public void UploadPixels<TPixel>(int level, Range xRange, Range yRange, Range zRange, PixelFormat format,
        PixelType type, Span<TPixel> pixels)
        where TPixel : unmanaged {
        (int xOffset, int width) = xRange.GetOffsetAndLength(Width);
        (int yOffset, int height) = yRange.GetOffsetAndLength(Height);
        (int zOffset, int depth) = zRange.GetOffsetAndLength(Depth);
        Gl.TextureSubImage3D(Handle, level, xOffset, yOffset, zOffset, (uint)width, (uint)height, (uint)depth, format,
            type, pixels.GetPinnableReference());
    }
}