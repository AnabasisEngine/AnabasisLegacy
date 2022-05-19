using Anabasis.Platform.Silk.Textures;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

public partial interface IGlApi
{
    TextureHandle CreateTexture(TextureTarget target);
    void BindTextureUnit(uint unit, TextureHandle handle);
    void DeleteTexture(TextureHandle handle);
    TextureHandle GenTexture();

    void TextureView(TextureHandle texture, TextureTarget target, TextureHandle origtexture,
        SizedInternalFormat internalformat, uint minlevel, uint numlevels, uint minlayer, uint numlayers);

    void TextureStorage1D(TextureHandle texture, uint levels, SizedInternalFormat internalformat, uint width);

    void TextureStorage2D(TextureHandle texture, uint levels, SizedInternalFormat internalformat, uint width,
        uint height);

    void TextureStorage3D(TextureHandle texture, uint levels, SizedInternalFormat internalformat, uint width,
        uint height, uint depth);

    void TextureSubImage1D<T0>(TextureHandle texture, int level, int xoffset, uint width, PixelFormat format,
        PixelType type, in T0 pixels)
        where T0 : unmanaged;

    void TextureSubImage2D<T0>(TextureHandle texture, int level, int xoffset, int yoffset, uint width, uint height,
        PixelFormat format, PixelType type, in T0 pixels)
        where T0 : unmanaged;

    void TextureSubImage3D<T0>(TextureHandle texture, int level, int xoffset, int yoffset, int zoffset, uint width,
        uint height, uint depth, PixelFormat format, PixelType type, in T0 pixels)
        where T0 : unmanaged;

    void GenerateTextureMipmap(TextureHandle texture);
}