namespace Anabasis.Tasks;

internal static class AnabasisTaskCompletionSourceCoreShared
{
    internal static readonly Action<object?> Sentinel = CompletionSentinel;

    private static void CompletionSentinel(object? _) // named method to aid debugging
    {
        throw new InvalidOperationException("The sentinel delegate should never be invoked.");
    }
}