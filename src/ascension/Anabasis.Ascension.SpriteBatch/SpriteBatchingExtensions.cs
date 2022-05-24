using Microsoft.Extensions.DependencyInjection;

namespace Anabasis.Ascension.SpriteBatch;

public static class SpriteBatchingExtensions
{
    public static void AddSpriteBatchSupport(this IServiceCollection services) =>
        services.AddSingleton<SpriteBatchSupport>();
}