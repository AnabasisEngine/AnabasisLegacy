using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Anabasis.Hosting.Hosting;

public static class AnabasisHostBuilderExtensions
{
    public static IHostBuilder ConfigureAnabasis(this IHostBuilder hostBuilder, string[]? args,
        Action<IHostBuilder>? configureHost = null) {
        configureHost?.Invoke(hostBuilder);

        return hostBuilder
            .ConfigureDefaults(args)
            .ConfigureLogging(logging => { })
            .ConfigureAppConfiguration(config => { })
            .ConfigureServices(services => {
                services.AddAnabasisCore();
                services.AddHostedService<AnabasisHostedService>();
                services.AddSingleton<IHostLifetime, AnabasisLifetime>();
            });
    }

    private static string[] GetCommandLineArguments() {
        var args = Environment.GetCommandLineArgs();
        return args.Any()
            ? args.Skip(1).ToArray() // args[0] is the path to executable binary.
            : Array.Empty<string>();
    }
}