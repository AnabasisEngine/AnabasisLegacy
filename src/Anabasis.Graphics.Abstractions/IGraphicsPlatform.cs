using Anabasis.Platform.Abstractions;

namespace Anabasis.Graphics.Abstractions;

public interface IGraphicsPlatform : IAnabasisPlatform
{
    /// <summary>
    /// The platform graphics device/context.
    /// Should be guaranteed non-null when <see cref="CreateGraphicsContext"/> returns.
    /// Will be registered as a singleton in the DI container, accessing before use of <see cref="CreateGraphicsContext"/> is an error.
    /// </summary>
    public IGraphicsDevice GraphicsDevice { get; }
}