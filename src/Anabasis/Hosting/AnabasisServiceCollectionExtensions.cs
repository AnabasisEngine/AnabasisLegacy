using Anabasis.Abstractions;
using Anabasis.Graphics.Abstractions;
using Anabasis.Internal;
using Anabasis.Platform.Abstractions;
using Anabasis.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Anabasis.Hosting;

public static class AnabasisServiceCollectionExtensions
{
    public static IServiceCollection AddAnabasisCore(this IServiceCollection services, string[] args) {
        services.TryAddSingleton(sp =>
            sp.GetRequiredService<IAnabasisPlatform>().Window ?? throw new InvalidOperationException());
        services.TryAddSingleton(sp =>
            sp.GetRequiredService<IGraphicsPlatform>().GraphicsDevice ?? throw new InvalidOperationException());
        services.TryAddSingleton<IAnabasisRunLoop, RunLoop>();
        services.TryAddSingleton<IAnabasisTime, GameTime>();
        services.TryAddSingleton<AnabasisTaskManager>();
        return services;
    }
}