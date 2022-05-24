using Microsoft.Extensions.DependencyInjection;

namespace Anabasis.Ascension.Tileset;

public static class TilesetExtensions
{
    public static void AddTilesetLoader(this IServiceCollection services) => services.AddScoped<TilesetLoader>();
}