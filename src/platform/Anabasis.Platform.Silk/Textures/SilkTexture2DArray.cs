using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Platform.Silk.Internal;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Textures;

public class SilkTexture2DArray : SilkTexture3D, ITexture2DArray
{
    public SilkTexture2DArray(IGlApi glApi, SizedInternalFormat format, int levels, int width, int height, int depth)
        : base(glApi, TextureTarget.Texture2DArray, format, levels, width, height, depth) { }

    private SilkTexture2DArray(IGlApi glApi, TextureHandle handle) : base(glApi, handle) { }

    public int Layers => Depth;

    public ITexture CreateView(Range levels, Range layers) {
        TextureHandle viewName = Gl.GenTexture();
        (int layerOffset, int layerLength) = layers.GetOffsetAndLength(Layers);
        (int levelOffset, int levelsLength) = levels.GetOffsetAndLength(MipmapLevels);

        if (layerLength == 1) {
            Gl.TextureView(viewName, TextureTarget.Texture2D, Handle,
                Format, (uint)levelOffset, (uint)levelsLength, (uint)layerOffset, (uint)layerLength);
            return new SilkTexture2D(Gl, viewName) {
                MipmapLevels = levelsLength,
                Format = Format,
                Width = ComputeMipmapDimension(levelOffset, Width),
                Height = ComputeMipmapDimension(levelOffset, Height),
            };
        } else {
            Gl.TextureView(viewName, TextureTarget.Texture2DArray, Handle,
                Format, (uint)levelOffset, (uint)levelsLength, (uint)layerOffset, (uint)layerLength);
            return new SilkTexture2DArray(Gl, viewName) {
                MipmapLevels = levelsLength,
                Format = Format,
                Depth = layerLength,
                Width = ComputeMipmapDimension(levelOffset, Width),
                Height = ComputeMipmapDimension(levelOffset, Height),
            };
        }
    }
}