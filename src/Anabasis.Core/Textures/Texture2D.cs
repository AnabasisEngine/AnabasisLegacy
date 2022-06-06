using Anabasis.Core.Handles;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Textures;

public class Texture2D : Texture
{
    public uint Width { get; }
    public uint Height { get; }

    public Texture2D(GL gl, TextureHandle name, uint width, uint height, uint mipmaps,
        SizedInternalFormat format) :
        base(gl, name, mipmaps, format) {
        Width = width;
        Height = height;
        Target = TextureTarget.Texture2D;
    }

    public Texture2D(GL gl, uint width, uint height, uint mipmaps,
        SizedInternalFormat format) : base(gl,
        TextureTarget.Texture2D, mipmaps, format) {
        Width = width;
        Height = height;
        gl.TextureStorage2D(Handle.Value, mipmaps, format, width, height);
        Gl.ThrowIfError(nameof(Gl.TextureStorage2D));
    }

    public void UploadPixels<TPixel>(int mipmapLevel, Box2D<int> rect, PixelFormat pixelFormat, PixelType pixelType,
        ReadOnlySpan<TPixel> pixels)
        where TPixel : unmanaged {
        Gl.TextureSubImage2D(Handle.Value, mipmapLevel, rect.Min.X, rect.Min.Y, (uint)rect.Size.X,
            (uint)rect.Size.Y, pixelFormat, pixelType, pixels);
        Gl.ThrowIfError(nameof(Gl.TextureSubImage2D));
    }
}