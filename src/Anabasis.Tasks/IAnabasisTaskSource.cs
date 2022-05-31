namespace Anabasis.Tasks;

public interface IAnabasisTaskSource
{
    AnabasisTaskStatus GetStatus(short token);
    void OnCompleted(Action<object?> continuation, object? state, short token);
    void GetResult(short token);
    
    
    AnabasisTaskStatus UnsafeGetStatus(); // only for debug use.
}


public interface IAnabasisTaskSource<out T> : IAnabasisTaskSource
{
    new T GetResult(short token);
}