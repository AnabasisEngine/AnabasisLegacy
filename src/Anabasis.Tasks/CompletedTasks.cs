namespace Anabasis.Tasks;

internal static class CompletedTasks
{
    public static readonly AnabasisTask<AsyncUnit> AsyncUnit = AnabasisTask.FromResult(Tasks.AsyncUnit.Default);
    public static readonly AnabasisTask<bool>      True      = AnabasisTask.FromResult(true);
    public static readonly AnabasisTask<bool>      False     = AnabasisTask.FromResult(false);
    public static readonly AnabasisTask<int>       Zero      = AnabasisTask.FromResult(0);
    public static readonly AnabasisTask<int>       MinusOne  = AnabasisTask.FromResult(-1);
    public static readonly AnabasisTask<int>       One       = AnabasisTask.FromResult(1);
}