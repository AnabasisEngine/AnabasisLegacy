using Anabasis.Threading;

// ReSharper disable CheckNamespace

namespace Anabasis.Abstractions;

public interface IAnabasisGame
{
    /// <remarks><see cref="AnabasisTaskManager.IsOnMainThread"/> is guaranteed to return true during the beginning of execution of this method</remarks>
    AnabasisCoroutine Load();

    void Update();

    /// <remarks><see cref="AnabasisTaskManager.IsOnMainThread"/> is guaranteed to return true during execution of this method</remarks>
    void Render();
}