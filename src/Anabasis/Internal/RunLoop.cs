using Anabasis.Platform.Abstractions;

namespace Anabasis.Internal;

internal sealed partial class RunLoop : IAnabasisRunLoop
{
    public void Load() {
        RunHandlers(AnabasisPlatformLoopStep.Initialization);
        RunHandlers(AnabasisPlatformLoopStep.PostInitialization);
    }

    public void Update() {
        RunHandlers(AnabasisPlatformLoopStep.PreUpdate);
        RunHandlers(AnabasisPlatformLoopStep.Update);
        RunHandlers(AnabasisPlatformLoopStep.PostUpdate);
    }

    public void Render() {
        RunHandlers(AnabasisPlatformLoopStep.PreRender);
        RunHandlers(AnabasisPlatformLoopStep.Render);
        RunHandlers(AnabasisPlatformLoopStep.PostRender);
    }
}