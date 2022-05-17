using Anabasis.Abstractions;

namespace Anabasis.Platform.Abstractions;

public interface IAnabasisRunLoop
{
    public IDisposable RegisterHandler(AnabasisPlatformLoopStep step, int priority, string name, Action handler);

    public void RemoveHandler(AnabasisPlatformLoopStep step, string name);

    public void Load();
    public void Update();
    public void Render();
}