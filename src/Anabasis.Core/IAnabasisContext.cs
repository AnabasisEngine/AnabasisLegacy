using Anabasis.Tasks;

namespace Anabasis.Core;

/// <summary>
/// A scene or a full game object, or generally anything that can (optionally asynchronously) load and/or 
/// </summary>
public interface IAnabasisContext
{
    AnabasisTask Load();
    void Update();
    void Render();
}