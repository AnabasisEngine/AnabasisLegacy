using Anabasis.Hosting.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Anabasis.Hosting;

public class AnabasisApp : IDisposable
{
    private readonly IHost _host;

    public AnabasisApp(IHost host) {
        _host = host;
    }

    public IServiceProvider Services => _host.Services;
    public IConfiguration Configuration => _host.Services.GetRequiredService<IConfiguration>();
    public IHostEnvironment Environment => _host.Services.GetRequiredService<IHostEnvironment>();
    public IHostApplicationLifetime Lifetime => _host.Services.GetRequiredService<IHostApplicationLifetime>();
    public ILogger Logger => _host.Services.GetRequiredService<ILogger<AnabasisApp>>();


    public Task RunAsync(CancellationToken cancellationToken = default)
        => _host.RunAsync(cancellationToken);

    public void Dispose()
        => _host.Dispose();

    public static AnabasisAppBuilder CreateBuilder(string[]? args = default,
        Action<AnabasisAppOptions>? configureOptions = null)
        => new AnabasisAppBuilder(args, configureOptions);
}