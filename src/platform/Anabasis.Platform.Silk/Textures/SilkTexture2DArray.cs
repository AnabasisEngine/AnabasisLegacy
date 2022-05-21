using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Platform.Abstractions;
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

    public ITextureView2D Upload(int layer) => new Texture2DArrayView(this, layer);

    private class Texture2DArrayView : ITextureView2D, ISupportRawPixelUpload2D<PixelFormat, PixelType>
    {
        private readonly SilkTexture2DArray _array;
        private readonly int                _layer;

        public Texture2DArrayView(SilkTexture2DArray array, int layer) {
            _array = array;
            _layer = layer;
        }

        public void UploadPixels(int level, Range xRange, Range yRange, ReadOnlySpan<Color> pixels) {
            _array.UploadPixels(level, xRange, yRange, _layer..(_layer + 1), pixels);
        }

        public int Width => _array.Width;
        public int Height => _array.Height;

        public void UploadPixels<TPixel>(int level, Range xRange, Range yRange, PixelFormat format, PixelType type,
            ReadOnlySpan<TPixel> pixels)
            where TPixel : unmanaged {
            _array.UploadPixels(level, xRange, yRange, _layer..(_layer + 1), format, type, pixels);
        }
    }
}