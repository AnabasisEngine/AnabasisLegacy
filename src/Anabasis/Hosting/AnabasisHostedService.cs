using Microsoft.Extensions.Hosting;

namespace Anabasis.Hosting;

public class AnabasisHostedService
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

    public async Task StopAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
}