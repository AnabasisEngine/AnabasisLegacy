using Anabasis.Hosting;
using Microsoft.Extensions.Hosting;

namespace Anabasis.Builder;

internal static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureDefaultAnabasis(this IHostBuilder hostBuilder,
        string[]? args /*, Action<ICoconaCommandsBuilder> configureApplication*/) {
        var builder = new AnabasisAppHostBuilder(hostBuilder);
        builder.ConfigureDefaults(args /*, configureApplication*/);

        return hostBuilder;
    }
}