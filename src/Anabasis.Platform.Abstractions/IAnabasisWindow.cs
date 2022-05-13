namespace Anabasis.Platform.Abstractions;

public interface IAnabasisWindow
{
    public void Run(IAnabasisRunLoop runLoop, IAnabasisTime time);

    public void Close();

    public Task WaitForShutdownAsync();
}