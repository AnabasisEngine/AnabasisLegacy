using Anabasis.Abstractions;
using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Threading;
using Microsoft.Extensions.Logging;

namespace BasicSample;

public class Game : IAnabasisGame
{
    private readonly ILogger<Game>       _logger;
    private readonly IAnabasisTime       _time;
    private readonly IGraphicsDevice     _graphics;
    private readonly ITextureSupport     _textureSupport;
    private readonly IShaderSupport      _shaderSupport;
    private readonly AnabasisTaskManager _taskManager;

    public Game(ILogger<Game> logger, IAnabasisTime time, IGraphicsDevice graphics, ITextureSupport textureSupport,
        IShaderSupport shaderSupport, AnabasisTaskManager taskManager) {
        _logger = logger;
        _time = time;
        _graphics = graphics;
        _textureSupport = textureSupport;
        _shaderSupport = shaderSupport;
        _taskManager = taskManager;
    }

    public async AnabasisCoroutine Load() {
        _logger.LogDebug("Game.Load");
        await _taskManager.Yield(AnabasisPlatformStepMask.PostInitialization);
        _logger.LogDebug("Game.Load in PostInitialization");
    }

    public void Update() { }

    public void Render() { }
}