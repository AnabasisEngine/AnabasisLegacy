using Anabasis.Tasks;

namespace Anabasis.Core;

/// <summary>
/// A scene or a full game object, or generally anything that can (optionally asynchronously) load and/or 
/// </summary>
public interface IAnabasisContext
{
    AnabasisTask LoadAsync();
    void Update();
    void Render();
}