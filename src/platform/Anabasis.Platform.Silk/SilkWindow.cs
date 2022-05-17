using Anabasis.Abstractions;
using Anabasis.Platform.Abstractions;
using Microsoft.Extensions.Hosting;
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

    public void Run(IAnabasisRunLoop runLoop, IAnabasisTime time) {
        Window.Load += () => {
            ((SilkGraphicsDevice)_silkPlatform.GraphicsDevice).Load();
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
        Window.Run();
        _tcs.TrySetResult();
        _applicationLifetime.StopApplication();
    }

    public void Close() {
        Window.Close();
    }

    public Task WaitForShutdownAsync() => _tcs.Task;
}