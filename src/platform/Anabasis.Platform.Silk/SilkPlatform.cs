using Anabasis.Platform.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Anabasis.Platform.Silk;

public sealed partial class SilkPlatform : IAnabasisPlatform
{
    public static void ConfigureServices(HostBuilderContext context, IServiceCollection services) {
        services.TryAddSingleton<IAnabasisPlatform, SilkPlatform>();
        services.TryAddScoped<SilkGraphicsDevice>();
        services.TryAddScoped<IGraphicsDevice>(s => s.GetRequiredService<SilkGraphicsDevice>());
    }

    public IAnabasisWindow CreateWindow() => new SilkWindow();
}