using Anabasis.Abstractions;
using Anabasis.Platform.Abstractions;
using Anabasis.Platform.Silk.Internal;
using Microsoft.Extensions.Hosting;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Anabasis.Platform.Silk;

internal class SilkWindow : IAnabasisWindow
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly SilkPlatform             _silkPlatform;
    private readonly TaskCompletionSource     _tcs;

    internal SilkWindow(IWindow window, IHostApplicationLifetime applicationLifetime, SilkPlatform silkPlatform) {
        _applicationLifetime = applicationLifetime;
        _silkPlatform = silkPlatform;
        Window = window;
        _tcs = new TaskCompletionSource();
    }

    internal IWindow Window { get; }

    public void Run(IAnabasisRunLoop runLoop, IAnabasisTime time, Action unloadCallback) {
        Window.Load += () => {
            IGlApi glApi = ((SilkGraphicsDevice)_silkPlatform.GraphicsDevice).Load();
            ((GlApi)glApi).Gl.Enable(EnableCap.DepthTest);
            runLoop.Load();
        };
        Window.Update += d => {
            time.Update(d);
            runLoop.Update();
        };
        Window.Render += d => {
            time.Render(d);
            runLoop.Render();
        };
        Window.FramebufferResize += v => _silkPlatform.GraphicsDevice.Viewport = v.As<uint>();
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