using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Anabasis.Platform.Abstractions;

public interface IAnabasisPlatform
{
    public IDisposable RegisterHandler(AnabasisPlatformLoopStep step, int priority, string name, Action handler);
    void RemoveHandler(AnabasisPlatformLoopStep step, string name);
    public static abstract void ConfigureServices(HostBuilderContext context, IServiceCollection services);

    public IAnabasisWindow CreateWindow();
}