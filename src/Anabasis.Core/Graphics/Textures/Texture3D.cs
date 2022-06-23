using Anabasis.Core.Graphics.Handles;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Textures;

public class Texture3D : Texture
{
    public uint Width { get; }
    public uint Height { get; }

    public Texture3D(GL api, TextureHandle name, uint width, uint height, uint depth, uint mipmaps,
        SizedInternalFormat format) : base(api, name, mipmaps, format) {
        Width = width;
        Height = height;
        Target = TextureTarget.Texture3D;
    }

    public Texture3D(GL api, uint width, uint height, uint depth, uint mipmaps,
        SizedInternalFormat format) : this(api, TextureTarget.Texture3D, width, height,
        depth, mipmaps, format) { }

    protected Texture3D(GL api, TextureTarget target, uint width, uint height, uint depth, uint mipmaps,
        SizedInternalFormat format) : base(api, target, mipmaps, format) {
        Width = width;
        Height = height;
        api.TextureStorage3D(Handle.Value, mipmaps, format, width, height, depth);
    }

    public void UploadPixels<TPixel>(int mipmapLevel, Box3D<int> rect, PixelFormat pixelFormat, PixelType pixelType,
        ReadOnlySpan<TPixel> pixels)
        where TPixel : unmanaged {
        Api.TextureSubImage3D(Handle.Value, mipmapLevel, rect.Min.X, rect.Min.Y, rect.Min.Z, (uint)rect.Size.X,
            (uint)rect.Size.Y, (uint)rect.Size.Z, pixelFormat, pixelType, pixels);
    }
}