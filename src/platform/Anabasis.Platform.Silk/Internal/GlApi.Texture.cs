using Anabasis.Platform.Silk.Textures;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

internal partial class GlApi
{
    public TextureHandle CreateTexture(TextureTarget target) {
        Gl.CreateTextures(target, 1, out uint value);
        return new TextureHandle(value);
    }

    public void BindTextureUnit(uint unit, TextureHandle handle) {
        Gl.BindTextureUnit(unit, handle.Value);
    }

    public void DeleteTexture(TextureHandle handle) {
        Gl.DeleteTexture(handle.Value);
    }

    public TextureHandle GenTexture() => new(Gl.GenTexture());

    public void TextureView(TextureHandle texture, TextureTarget target, TextureHandle origtexture,
        SizedInternalFormat internalformat,
        uint minlevel, uint numlevels, uint minlayer, uint numlayers) {
        Gl.TextureView(texture.Value, target, origtexture.Value, internalformat, minlevel, numlevels, minlayer,
            numlayers);
    }

    public void TextureStorage1D(TextureHandle texture, uint levels, SizedInternalFormat internalformat, uint width) {
        Gl.TextureStorage1D(texture.Value, levels, internalformat, width);
    }

    public void TextureStorage2D(TextureHandle texture, uint levels, SizedInternalFormat internalformat, uint width,
        uint height) {
        Gl.TextureStorage2D(texture.Value, levels, internalformat, width, height);
    }

    public void TextureStorage3D(TextureHandle texture, uint levels, SizedInternalFormat internalformat, uint width,
        uint height,
        uint depth) {
        Gl.TextureStorage3D(texture.Value, levels, internalformat, width, height, depth);
    }

    public void TextureSubImage1D<T0>(TextureHandle texture, int level, int xoffset, uint width, PixelFormat format,
        PixelType type,
        in T0 pixels)
        where T0 : unmanaged {
        Gl.TextureSubImage1D(texture.Value, level, xoffset, width, format, type, in pixels);
    }

    public void TextureSubImage2D<T0>(TextureHandle texture, int level, int xoffset, int yoffset, uint width,
        uint height,
        PixelFormat format, PixelType type, in T0 pixels)
        where T0 : unmanaged {
        Gl.TextureSubImage2D(texture.Value, level, xoffset, yoffset, width, height, format, type, in pixels);
    }

    public void TextureSubImage3D<T0>(TextureHandle texture, int level, int xoffset, int yoffset, int zoffset,
        uint width, uint height,
        uint depth, PixelFormat format, PixelType type, in T0 pixels)
        where T0 : unmanaged {
        Gl.TextureSubImage3D(texture.Value, level, xoffset, yoffset, zoffset, width, height, depth, format, type, in
            pixels);
    }

    public void GenerateTextureMipmap(TextureHandle texture) => Gl.GenerateTextureMipmap(texture.Value);
}