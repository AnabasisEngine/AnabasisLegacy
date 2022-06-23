using Anabasis.Core.Graphics.Handles;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Textures;

public class Texture2DArray : Texture3D
{
    public Texture2DArray(GL api, TextureHandle name, uint width, uint height, uint depth, uint mipmaps,
        SizedInternalFormat format) :
        base(api, name, width, height, depth, mipmaps, format) {
        Target = TextureTarget.Texture2DArray;
    }

    public Texture2DArray(GL api, uint width, uint height, uint depth, uint mipmaps,
        SizedInternalFormat format) : base(api, TextureTarget.Texture2DArray, width, height,
        depth, mipmaps, format) { }

    public Texture CreateView(uint minlevel, uint numlevels, uint minlayer, uint numlayers) {
        uint handle = Api.GenTexture();
        if (numlayers == 1) {
            Api.TextureView(handle, TextureTarget.Texture2D, Handle.Value, Format, minlevel, numlevels, minlayer,
                numlayers);
            return new Texture2D(Api, new TextureHandle(handle), Width, Height, numlevels, Format);
        } else {
            Api.TextureView(handle, TextureTarget.Texture2DArray, Handle.Value, Format, minlevel, numlevels, minlayer,
                numlayers);
            return new Texture2DArray(Api, new TextureHandle(handle), Width, Height, numlayers, numlevels, Format);
        }
    }

    public void UploadPixels<TPixel>(int mipmapLevel, Box2D<int> rect, int layer, PixelFormat pixelFormat,
        PixelType pixelType, ReadOnlySpan<TPixel> pixels)
        where TPixel : unmanaged {
        Api.TextureSubImage3D(Handle.Value, mipmapLevel, rect.Min.X, rect.Min.Y, layer, (uint)rect.Size.X,
            (uint)rect.Size.Y, 1, pixelFormat, pixelType, pixels);
    }
}