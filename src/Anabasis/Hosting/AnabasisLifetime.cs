using Anabasis.Platform.Abstractions;
using Microsoft.Extensions.Hosting;

namespace Anabasis.Hosting;

internal class AnabasisLifetime : IHostLifetime
{
    public AnabasisLifetime(IHostApplicationLifetime applicationLifetime, IAnabasisPlatform anabasisPlatform) {
        ApplicationLifetime = applicationLifetime;
        AnabasisPlatform = anabasisPlatform;
    }
    public IHostApplicationLifetime ApplicationLifetime { get; }
    public IAnabasisPlatform AnabasisPlatform { get; }
    
    public Task WaitForStartAsync(CancellationToken cancellationToken) {
        AnabasisPlatform.Initialize();
        Window = AnabasisPlatform.Window;
        return Task.CompletedTask;
    }

    public IAnabasisWindow Window { get; set; } = null!;

    public Task StopAsync(CancellationToken cancellationToken) {
        Window.Close();
        return Window.WaitForShutdownAsync();
    }
}