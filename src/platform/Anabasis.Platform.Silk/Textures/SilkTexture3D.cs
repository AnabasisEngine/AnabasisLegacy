using System.Runtime.CompilerServices;
using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Platform.Silk.Internal;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Textures;

public class SilkTexture3D : SilkTexture, ITexture3D, ISupportRawPixelUpload3D<PixelFormat, PixelType>
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

    protected SilkTexture3D(IGlApi glApi, TextureHandle handle, TextureTarget target) : base(glApi, handle, target) { }
    public int Width { get; protected init; }
    public int Height { get; protected init; }
    public int Depth { get; protected init; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UploadPixels(int level, int xOffset, uint width, int yOffset, uint height, int zOffset, uint depth,
        ReadOnlySpan<Color> pixels) =>
        UploadPixels(level, xOffset, width, yOffset, height, zOffset, depth, PixelFormat.Rgba, PixelType.UnsignedByte,
            pixels);

    public void UploadPixels<TPixel>(int level, int xOffset, uint width, int yOffset, uint height, int zOffset,
        uint depth,
        PixelFormat format, PixelType type, ReadOnlySpan<TPixel> pixels)
        where TPixel : unmanaged {
        Gl.TextureSubImage3D(Handle, level, xOffset, yOffset, zOffset, width, height, depth, format, type,
            pixels.GetPinnableReference());
    }
}