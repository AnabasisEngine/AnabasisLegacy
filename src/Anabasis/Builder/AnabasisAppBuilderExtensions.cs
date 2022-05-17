using Anabasis.Platform.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Anabasis.Builder;

public static class AnabasisAppBuilderExtensions
{
    public static AnabasisAppBuilder ConfigureServices(this AnabasisAppBuilder builder,
        Action<IServiceCollection> configureDelegate) =>
        builder.ConfigureServices((_, _, services) => configureDelegate(services));

    public static AnabasisAppBuilder ConfigureServices(this AnabasisAppBuilder builder,
        Action<AnabasisAppBuilder, IServiceCollection> configureDelegate) =>
        builder.ConfigureServices((appBuilder, _, services) => configureDelegate(appBuilder, services));

    public static AnabasisAppBuilder ConfigureServices(this AnabasisAppBuilder builder,
        Action<AnabasisAppBuilder, HostBuilderContext, IServiceCollection> configureDelegate) {
        builder.Host.ConfigureServices((context, collection) => configureDelegate(builder, context, collection));
        return builder;
    }

    public static AnabasisAppBuilder ConfigureServices(this AnabasisAppBuilder builder,
        Action<HostBuilderContext, IServiceCollection> configureDelegate) {
        builder.Host.ConfigureServices(configureDelegate);
        return builder;
    }

    public static AnabasisAppBuilder UsingPlatform<TPlatform>(this AnabasisAppBuilder builder)
        where TPlatform : class, IAnabasisPlatform =>
        builder.ConfigureServices(TPlatform.ConfigureServices);
}