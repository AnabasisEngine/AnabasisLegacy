using Anabasis.Core.Handles;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Textures;

public class Texture2DArray : Texture3D
{
    public Texture2DArray(GL gl, TextureHandle name, uint width, uint height, uint depth, uint mipmaps,
        SizedInternalFormat format) :
        base(gl, name, width, height, depth, mipmaps, format) {
        Target = TextureTarget.Texture2DArray;
    }

    public Texture2DArray(GL gl, uint width, uint height, uint depth, uint mipmaps,
        SizedInternalFormat format) : base(gl, TextureTarget.Texture2DArray, width, height,
        depth, mipmaps, format) { }

    public Texture CreateView(uint minlevel, uint numlevels, uint minlayer, uint numlayers) {
        uint handle = Gl.GenTexture();
        if (numlayers == 1) {
            Gl.TextureView(handle, TextureTarget.Texture2D, Handle.Value, Format, minlevel, numlevels, minlayer,
                numlayers);
            return new Texture2D(Gl, new TextureHandle(handle), Width, Height, numlevels, Format);
        } else {
            Gl.TextureView(handle, TextureTarget.Texture2DArray, Handle.Value, Format, minlevel, numlevels, minlayer,
                numlayers);
            return new Texture2DArray(Gl, new TextureHandle(handle), Width, Height, numlayers, numlevels, Format);
        }
    }

    public void UploadPixels<TPixel>(int mipmapLevel, Box2D<int> rect, int layer, PixelFormat pixelFormat,
        PixelType pixelType, ReadOnlySpan<TPixel> pixels)
        where TPixel : unmanaged {
        Gl.TextureSubImage3D(Handle.Value, mipmapLevel, rect.Min.X, rect.Min.Y, layer, (uint)rect.Size.X,
            (uint)rect.Size.Y, 1, pixelFormat, pixelType, pixels);
    }
}