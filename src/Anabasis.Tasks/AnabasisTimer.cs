namespace Anabasis.Tasks;

public sealed class AnabasisTimer : IDisposable
{
    private readonly PeriodicTimer _timer;

    public AnabasisTimer(TimeSpan period) {
        _timer = new PeriodicTimer(period);
    }
    
    public async AnabasisTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default) {
        try {
            return await _timer.WaitForNextTickAsync(cancellationToken); }
        finally {
            await AnabasisTask.SwitchToMainThread(cancellationToken: cancellationToken);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _timer.Dispose();
    }

    ~AnabasisTimer() => Dispose();
}