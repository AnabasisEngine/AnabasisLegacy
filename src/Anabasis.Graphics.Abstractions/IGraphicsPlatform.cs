using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Platform.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Anabasis.Graphics.Abstractions;

public interface IGraphicsPlatform : IAnabasisPlatform
{
    /// <summary>
    /// The platform graphics device/context.
    /// Should be guaranteed non-null when <see cref="IAnabasisPlatform.Initialize"/> returns.
    /// Will be registered as a singleton in the DI container, accessing before use of <see cref="IAnabasisPlatform.Initialize"/> is an error.
    /// </summary>
    public IGraphicsDevice GraphicsDevice { get; }

    /// <inheritdoc cref="IAnabasisPlatform"/>
    /// <summary>
    /// In addition to the services required by <see cref="IAnabasisPlatform"/>, this should register:
    ///
    /// - A scoped <see cref="IShaderSupport"/>
    /// - A scoped <see cref="ITextureSupport"/>
    /// Any scoped services may assume that <see cref="IAnabasisPlatform.Initialize"/> has run to completion exactly once 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="services"></param>
    public new static abstract void ConfigureServices(HostBuilderContext context, IServiceCollection services);
}