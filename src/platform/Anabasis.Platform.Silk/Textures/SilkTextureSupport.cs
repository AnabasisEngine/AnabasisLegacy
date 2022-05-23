using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Platform.Silk.Internal;
using Anabasis.Threading;
using Anabasis.Utility;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Textures;

public class SilkTextureSupport : ITextureSupport, ISupportCopyTextureData
{
    private readonly AnabasisTaskManager _taskManager;
    private readonly IGlApi              _gl;

    public SilkTextureSupport(AnabasisTaskManager taskManager, IGraphicsDevice graphicsDevice) {
        _taskManager = taskManager;
        _gl = Guard.IsType<SilkGraphicsDevice>(graphicsDevice).Gl;
    }

    public ValueTask<ITexture2D> CreateTexture2DAsync(int levels, int width, int height) =>
        _taskManager.RunOnGraphicsThread<ITexture2D>(() => new SilkTexture2D(
            _gl, SizedInternalFormat.Rgba8, levels, width, height));

    public ValueTask<ITexture3D> CreateTexture3DAsync(int levels, int width, int height,
        int depth) => _taskManager.RunOnGraphicsThread<ITexture3D>(() =>
        new SilkTexture3D(_gl, SizedInternalFormat.Rgba8, levels, width, height,
            depth));

    public ValueTask<ITexture2DArray> CreateTexture2DArrayAsync(int levels, int width,
        int height, int layers) => _taskManager.RunOnGraphicsThread<ITexture2DArray>(() =>
        new SilkTexture2DArray(_gl, SizedInternalFormat.Rgba8, levels, width,
            height, layers));

    public void CopyPixels(ITexture source, ITexture dest, int srcX, int srcY, int srcZ, int srcLevel, int destX,
        int destY, int destZ, int destLevel, uint srcWidth, uint srcHeight, uint srcDepth) {
        SilkTexture src = Guard.IsType<SilkTexture>(source);
        SilkTexture dst = Guard.IsType<SilkTexture>(dest);
        _gl.CopyImageSubData(src.Handle, src.Target, srcLevel, srcX, srcY, srcZ, dst.Handle, dst.Target, destLevel,
            destX, destY, destZ, srcWidth, srcHeight, srcDepth);
    }
}