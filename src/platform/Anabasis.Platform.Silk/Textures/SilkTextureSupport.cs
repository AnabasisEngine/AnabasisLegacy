using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Threading;
using Anabasis.Utility;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Textures;

public class SilkTextureSupport : ITextureSupport
{
    private readonly AnabasisTaskManager _taskManager;

    public SilkTextureSupport(AnabasisTaskManager taskManager) {
        _taskManager = taskManager;
    }

    public ValueTask<ITexture2D> CreateTexture2DAsync(IGraphicsDevice device, int levels, int width, int height) =>
        _taskManager.RunOnGraphicsThread<ITexture2D>(() => new SilkTexture2D(
            Guard.IsType<SilkGraphicsDevice>(device).Gl, SizedInternalFormat.Rgba8, levels, width, height));

    public ValueTask<ITexture3D> CreateTexture3DAsync(IGraphicsDevice device, int levels, int width, int height,
        int depth) => _taskManager.RunOnGraphicsThread<ITexture3D>(() =>
        new SilkTexture3D(Guard.IsType<SilkGraphicsDevice>(device).Gl, SizedInternalFormat.Rgba8, levels, width, height,
            depth));

    public ValueTask<ITexture2DArray> CreateTexture2DArrayAsync(IGraphicsDevice device, int levels, int width,
        int height, int layers) => _taskManager.RunOnGraphicsThread<ITexture2DArray>(() =>
        new SilkTexture2DArray(Guard.IsType<SilkGraphicsDevice>(device).Gl, SizedInternalFormat.Rgba8, levels, width,
            height, layers));
}