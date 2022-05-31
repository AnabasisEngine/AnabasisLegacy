using Anabasis.Tasks;

namespace Anabasis.Core;

public sealed partial class AnabasisRunLoop
{
    internal void Load() {
        RunHandlers(AnabasisPlatformLoopStep.Initialization);
        RunHandlers(AnabasisPlatformLoopStep.PostInitialization);
    }

    internal void Update() {
        RunHandlers(AnabasisPlatformLoopStep.PreUpdate);
        RunHandlers(AnabasisPlatformLoopStep.Update);
        RunHandlers(AnabasisPlatformLoopStep.PostUpdate);
    }

    internal void Render() {
        RunHandlers(AnabasisPlatformLoopStep.PreRender);
        RunHandlers(AnabasisPlatformLoopStep.Render);
        RunHandlers(AnabasisPlatformLoopStep.PostRender);
    }
}