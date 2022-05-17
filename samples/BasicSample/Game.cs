using Anabasis.Abstractions;
using Anabasis.Graphics.Abstractions;
using Microsoft.Extensions.Logging;

namespace BasicSample;

public class Game : IAnabasisGame
{
    private readonly ILogger<Game>   _logger;
    private readonly IAnabasisTime   _time;
    private readonly IGraphicsDevice _graphics;

    public Game(ILogger<Game> logger, IAnabasisTime time, IGraphicsDevice graphics) {
        _logger = logger;
        _time = time;
        _graphics = graphics;
    }

    public void Load() {
        _logger.LogDebug("Game.Load");
    }

    public void Update() {
    }

    public void Render() {
    }
}