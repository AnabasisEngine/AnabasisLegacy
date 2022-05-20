namespace Anabasis.Abstractions;

public interface IAnabasisRunLoop
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="step"></param>
    /// <param name="priority">The priority at which to register this handler. Greater values of <paramref name="priority"/> run earlier</param>
    /// <param name="name"></param>
    /// <param name="handler"></param>
    /// <remarks>
    /// The implementation of <see cref="IAnabasisGame"/> registered with the dependency injection container
    /// will have its methods registered with priority 0.
    /// </remarks>
    /// <returns></returns>
    public IDisposable RegisterHandler(AnabasisPlatformLoopStep step, int priority, string name, Action handler);

    public void RemoveHandler(AnabasisPlatformLoopStep step, string name);

    public void Load();
    public void Update();
    public void Render();
}