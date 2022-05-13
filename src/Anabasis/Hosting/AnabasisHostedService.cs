using Anabasis.Platform.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Anabasis.Hosting;

public class AnabasisHostedService : IHostedService
{
    private readonly IServiceProvider  _provider;
    private readonly IAnabasisPlatform _platform;
    private readonly Thread            _thread;

    public AnabasisHostedService(IServiceProvider provider, IAnabasisPlatform platform) {
        _provider = provider;
        _platform = platform;
        _thread = new Thread(RunDelegate) {
            Name = "Anabasis Thread",
        };
        _thread.TrySetApartmentState(ApartmentState.STA);
    }


    public Task StartAsync(CancellationToken cancellationToken) {
        _thread.Start(this);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        _platform.Window.Close();
        return Task.CompletedTask;
    }

    private static readonly ParameterizedThreadStart RunDelegate = o => {
        if (o is AnabasisHostedService hostedService)
            hostedService.Run();
    };


    private void Run() {
        _platform.Window.Run(_provider.GetRequiredService<IAnabasisRunLoop>(),
            _provider.GetRequiredService<IAnabasisTime>());
    }
}