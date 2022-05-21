using Anabasis.Abstractions;

namespace Anabasis.Platform.Abstractions;

public interface IAnabasisWindow
{
    public void Run(IAnabasisRunLoop runLoop, IAnabasisTime time, Action unloadCallback);

    public void Close();

    public Task WaitForShutdownAsync();
}