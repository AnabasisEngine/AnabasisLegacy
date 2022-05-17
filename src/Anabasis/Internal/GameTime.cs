using Anabasis.Abstractions;
using Microsoft.Extensions.Logging;

namespace Anabasis.Internal;

public class GameTime : IAnabasisTime
{
    private ILogger<GameTime> _logger;
    public GameTime(ILogger<GameTime> logger) {
        _logger = logger;
    }

    public void Update(double timeSinceLastUpdate) {
        // _logger.LogDebug("Update timing {Elapsed}", timeSinceLastUpdate);
    }

    public void Render(double timeSinceLastRender) {
        // _logger.LogDebug("Render timing {Elapsed}", timeSinceLastRender);
    }
}