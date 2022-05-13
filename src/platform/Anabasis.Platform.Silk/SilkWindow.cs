using Anabasis.Platform.Abstractions;
using Microsoft.Extensions.Hosting;
using Silk.NET.Windowing;

namespace Anabasis.Platform.Silk;

public class SilkWindow : IAnabasisWindow
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly TaskCompletionSource     _tcs;

    internal SilkWindow(IWindow window, IHostApplicationLifetime applicationLifetime) {
        _applicationLifetime = applicationLifetime;
        Window = window;
        _tcs = new TaskCompletionSource();
    }

    internal IWindow Window { get; }

    public void Run(IAnabasisRunLoop runLoop, IAnabasisTime time) {
        Window.Load += runLoop.Load;
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
        Window.Invoke(new Action<IWindow>(w => w.Close()), Window);
    }

    public Task WaitForShutdownAsync() => _tcs.Task;
}