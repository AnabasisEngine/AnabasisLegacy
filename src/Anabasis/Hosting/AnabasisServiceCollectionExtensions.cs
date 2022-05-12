using Anabasis.Platform.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Anabasis.Hosting;

public static class AnabasisServiceCollectionExtensions
{
    public static IServiceCollection AddAnabasisCore(this IServiceCollection services, string[] args) {
        services.TryAddScoped(sp => sp.GetRequiredService<IAnabasisPlatform>().CreateWindow());

        return services;
    }
}