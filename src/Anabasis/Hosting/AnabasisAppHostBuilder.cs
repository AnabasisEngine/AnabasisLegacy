using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Anabasis.Hosting;

public class AnabasisAppHostBuilder
{
    private readonly IHostBuilder _builder;

    public AnabasisAppHostBuilder(IHostBuilder hostBuilder) {
        _builder = hostBuilder;
    }

    /// <summary>
    /// Adds services to the container. See also <seealso cref="HostingHostBuilderExtensions.ConfigureServices(IHostBuilder, Action{IServiceCollection})"/>.
    /// </summary>
    /// <param name="configureDelegate"></param>
    /// <returns></returns>
    public AnabasisAppHostBuilder ConfigureServices(Action<IServiceCollection> configureDelegate) {
        _builder.ConfigureServices(configureDelegate);
        return this;
    }

    /// <summary>
    /// Adds services to the container. See also <seealso cref="HostingHostBuilderExtensions.ConfigureServices(IHostBuilder, Action{IServiceCollection})"/>.
    /// </summary>
    /// <param name="configureDelegate"></param>
    /// <returns></returns>
    public AnabasisAppHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate) {
        _builder.ConfigureServices(configureDelegate);
        return this;
    }

    /// <summary>
    /// Sets up the configuration for the application. See also <seealso cref="HostingHostBuilderExtensions.ConfigureAppConfiguration(IHostBuilder, Action{IConfigurationBuilder})"/>.
    /// </summary>
    /// <param name="configureDelegate"></param>
    /// <returns></returns>
    public AnabasisAppHostBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configureDelegate) {
        _builder.ConfigureAppConfiguration(configureDelegate);
        return this;
    }

    /// <summary>
    /// Sets up the configuration for the application. See also <seealso cref="HostingHostBuilderExtensions.ConfigureAppConfiguration(IHostBuilder, Action{IConfigurationBuilder})"/>.
    /// </summary>
    /// <param name="configureDelegate"></param>
    /// <returns></returns>
    public AnabasisAppHostBuilder ConfigureAppConfiguration(
        Action<HostBuilderContext, IConfigurationBuilder> configureDelegate) {
        _builder.ConfigureAppConfiguration(configureDelegate);
        return this;
    }

    /// <summary>
    /// Adds a delegate for configuring the provided <see cref="ILoggingBuilder"/>. This may be called multiple times. See also <seealso cref="HostingHostBuilderExtensions.ConfigureLogging(Microsoft.Extensions.Hosting.IHostBuilder,System.Action{Microsoft.Extensions.Logging.ILoggingBuilder})"/>.
    /// </summary>
    /// <param name="configureLogging"></param>
    /// <returns></returns>
    public AnabasisAppHostBuilder ConfigureLogging(Action<ILoggingBuilder> configureLogging) {
        _builder.ConfigureLogging(configureLogging);
        return this;
    }

    /// <summary>
    /// Adds a delegate for configuring the provided <see cref="ILoggingBuilder"/>. This may be called multiple times. See also <seealso cref="HostingHostBuilderExtensions.ConfigureLogging(Microsoft.Extensions.Hosting.IHostBuilder,System.Action{Microsoft.Extensions.Hosting.HostBuilderContext,Microsoft.Extensions.Logging.ILoggingBuilder})"/>.
    /// </summary>
    /// <param name="configureLogging"></param>
    /// <returns></returns>
    public AnabasisAppHostBuilder ConfigureLogging(Action<HostBuilderContext, ILoggingBuilder> configureLogging) {
        _builder.ConfigureLogging(configureLogging);
        return this;
    }

    internal void ConfigureDefaults(string[]? args) {
        _builder
            .ConfigureAnabasis(args);
    }
}