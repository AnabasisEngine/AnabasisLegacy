using System.Runtime.CompilerServices;

namespace Anabasis.Tasks;

public static class AnabasisTaskStatusExtensions
{
    /// <summary>status != Pending.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompleted(this AnabasisTaskStatus status) => status != AnabasisTaskStatus.Pending;

    /// <summary>status == Succeeded.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompletedSuccessfully(this AnabasisTaskStatus status) => status == AnabasisTaskStatus.Succeeded;

    /// <summary>status == Canceled.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCanceled(this AnabasisTaskStatus status) => status == AnabasisTaskStatus.Canceled;

    /// <summary>status == Faulted.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFaulted(this AnabasisTaskStatus status) => status == AnabasisTaskStatus.Faulted;
}