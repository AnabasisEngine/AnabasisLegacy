using Anabasis.Tasks;

namespace Anabasis.Core;

public abstract class AnabasisGame
    : IAnabasisContext
{
    protected Task LoadTask { get; private set; } = Task.CompletedTask;

    internal void DoLoad() {
        LoadTask = Load().AsTask();
    }
    public abstract AnabasisTask Load();
    public abstract void Update();
    public abstract void Render();
}