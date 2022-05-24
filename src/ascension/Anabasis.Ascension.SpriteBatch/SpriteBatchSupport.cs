using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Shaders;
using Microsoft.Extensions.Options;

namespace Anabasis.Ascension.SpriteBatch;

public sealed class SpriteBatchSupport
{
    private readonly IGraphicsDevice              _graphicsDevice;
    private readonly IShaderSupport               _shaderSupport;
    private readonly IOptions<SpriteBatchOptions> _options;

    public SpriteBatchSupport(IGraphicsDevice graphicsDevice, IShaderSupport shaderSupport,
        IOptions<SpriteBatchOptions> options) {
        _graphicsDevice = graphicsDevice;
        _shaderSupport = shaderSupport;
        _options = options;
    }

    public ValueTask<SpriteBatcher> CreateBatcher() =>
        SpriteBatcher.CreateSpriteBatcher(_graphicsDevice, _shaderSupport, _options);
}