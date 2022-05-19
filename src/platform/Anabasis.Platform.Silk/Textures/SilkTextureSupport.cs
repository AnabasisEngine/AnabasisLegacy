using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Utility;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Textures;

public class SilkTextureSupport : ITextureSupport
{
    public ITexture2D CreateTexture2D(IGraphicsDevice device, int levels, int width, int height) =>
        new SilkTexture2D(Guard.IsType<SilkGraphicsDevice>(device).Gl, SizedInternalFormat.Rgba8, levels, width,
            height);

    public ITexture3D CreateTexture3D(IGraphicsDevice device, int levels, int width, int height, int depth) =>
        new SilkTexture3D(Guard.IsType<SilkGraphicsDevice>(device).Gl, SizedInternalFormat.Rgba8, levels, width,
            height, depth);

    public ITexture2DArray
        CreateTexture2DArray(IGraphicsDevice device, int levels, int width, int height, int layers) =>
        new SilkTexture2DArray(Guard.IsType<SilkGraphicsDevice>(device).Gl, SizedInternalFormat.Rgba8, levels, width,
            height, layers);
}