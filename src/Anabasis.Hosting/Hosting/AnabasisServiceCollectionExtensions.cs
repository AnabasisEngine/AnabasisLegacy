using Anabasis.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;

namespace Anabasis.Hosting.Hosting;

public static class AnabasisServiceCollectionExtensions
{
    public static IServiceCollection AddAnabasisCore(this IServiceCollection services) {
        services.TryAddSingleton<AnabasisRunLoop>();
        services.AddOptions<AnabasisWindowingOptions>();
        services.TryAddSingleton(s => {
            AnabasisWindowingOptions options = s.GetRequiredService<IOptions<AnabasisWindowingOptions>>().Value;
            WindowOptions o = WindowOptions.Default;
            o.Size = options.Size;
            o.API = new GraphicsAPI(ContextAPI.OpenGL, new APIVersion(4, 6));
            GlfwWindowing.Use();
            return Window.Create(o);
        });
        services.TryAddSingleton<AnabasisWindow>();
        services.TryAddSingleton<AnabasisGraphicsDevice>();
        services.TryAddScoped(s => s.GetRequiredService<AnabasisGraphicsDevice>().GL);
        return services;
    }
}