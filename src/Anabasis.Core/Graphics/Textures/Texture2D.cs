using Anabasis.Core.Graphics.Handles;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Textures;

public class Texture2D : Texture
{
    public uint Width { get; }
    public uint Height { get; }

    public Texture2D(GL api, TextureHandle name, uint width, uint height, uint mipmaps,
        SizedInternalFormat format) :
        base(api, name, mipmaps, format) {
        Width = width;
        Height = height;
        Target = TextureTarget.Texture2D;
    }

    public Texture2D(GL api, uint width, uint height, uint mipmaps,
        SizedInternalFormat format) : base(api,
        TextureTarget.Texture2D, mipmaps, format) {
        Width = width;
        Height = height;
        api.TextureStorage2D(Handle.Value, mipmaps, format, width, height);
        Api.ThrowIfError(nameof(Api.TextureStorage2D));
    }

    public void UploadPixels<TPixel>(int mipmapLevel, Box2D<int> rect, PixelFormat pixelFormat, PixelType pixelType,
        ReadOnlySpan<TPixel> pixels)
        where TPixel : unmanaged {
        Api.TextureSubImage2D(Handle.Value, mipmapLevel, rect.Min.X, rect.Min.Y, (uint)rect.Size.X,
            (uint)rect.Size.Y, pixelFormat, pixelType, pixels);
        Api.ThrowIfError(nameof(Api.TextureSubImage2D));
    }
}