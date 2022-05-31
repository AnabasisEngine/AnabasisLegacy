using Microsoft.Extensions.Hosting;

namespace Anabasis.Hosting.Hosting;

internal class AnabasisLifetime : IHostLifetime
{
    public AnabasisLifetime(IHostApplicationLifetime applicationLifetime/*, IAnabasisPlatform anabasisPlatform*/) {
        ApplicationLifetime = applicationLifetime;
        // AnabasisPlatform = anabasisPlatform;
    }

    public IHostApplicationLifetime ApplicationLifetime { get; }
    // public IAnabasisPlatform AnabasisPlatform { get; }

    public Task WaitForStartAsync(CancellationToken cancellationToken) {
        // AnabasisPlatform.Initialize();
        // Window = AnabasisPlatform.Window;
        return Task.CompletedTask;
    }

    // public IAnabasisWindow Window { get; set; } = null!;

    public async Task StopAsync(CancellationToken cancellationToken) {
        // Window.Close();
        // await Window.WaitForShutdownAsync();
    }
}