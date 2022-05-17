using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Anabasis.Platform.Abstractions;

public interface IAnabasisPlatform
{

    /// <summary>
    /// This function is expected to register the following services, in addition to any required for the implementation:
    /// - A singleton <see cref="IAnabasisPlatform"/> - this should probably be an instance of the type on which this function was called
    ///
    /// Any scoped services may assume that <see cref="CreateGraphicsContext"/> has already been called
    /// </summary>
    public static abstract void ConfigureServices(HostBuilderContext context, IServiceCollection services);


    /// <summary>
    /// Initializes the basic graphics context and window. If this method completes without exception,
    /// then all services required for the initialization of the scoped <see cref="IGraphicsDevice"/>
    /// and <see cref="IAnabasisWindow"/> are available, if those services are not themselves already initialized by this
    /// method. Actually running the game loop infrastructure is implemented in <see cref="IAnabasisWindow.Run"/>
    /// </summary>
    public void CreateGraphicsContext();

    /// <summary>
    /// The platform window/view.
    /// Should be guaranteed non-null when <see cref="CreateGraphicsContext"/> returns.
    /// Will be registered as a singleton in the DI container, accessing before use of <see cref="CreateGraphicsContext"/> is an error.
    /// </summary>
    public IAnabasisWindow Window { get; }
}