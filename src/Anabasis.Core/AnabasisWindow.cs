using Microsoft.Extensions.Hosting;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Anabasis.Core;

public sealed class AnabasisWindow
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly TaskCompletionSource     _tcs;

    public AnabasisWindow(IWindow window, IHostApplicationLifetime applicationLifetime) {
        _applicationLifetime = applicationLifetime;
        Window = window;
        _tcs = new TaskCompletionSource();
    }

    internal IWindow Window { get; }

    public void Run(AnabasisRunLoop runLoop, AnabasisGraphicsDevice graphicsDevice, Action unloadCallback) {
        Window.Load += () => {
            graphicsDevice.Gl = Window.CreateOpenGL();
            graphicsDevice.Gl.Enable(EnableCap.DepthTest);
            runLoop.Load();
        };
        Window.Update += d => {
            runLoop.Update();
        };
        Window.Render += d => {
            runLoop.Render();
        };
        Window.FramebufferResize += v => graphicsDevice.ViewportSize = v;
        Window.Closing += unloadCallback;
        Window.Run();
        _tcs.TrySetResult();
        _applicationLifetime.StopApplication();
    }

    public void Close() {
        Window.Close();
    }

    public Task WaitForShutdownAsync() => _tcs.Task;
}