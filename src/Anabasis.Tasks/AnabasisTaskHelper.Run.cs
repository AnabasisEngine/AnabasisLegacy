using Microsoft.CodeAnalysis.PooledObjects;

namespace Anabasis.Tasks;

public static partial class AnabasisTaskHelper
{
    public static void Schedule(AnabasisPlatformLoopStep timing, Action continuation) {
        Schedule(timing.ToMask(), continuation);
    }

    public static void Schedule(AnabasisPlatformStepMask timing, Action continuation) {
        ScheduledContinuation scheduledContinuation = new(timing, continuation);
        Schedule(scheduledContinuation);
    }

    private static void Schedule(ScheduledContinuation scheduledContinuation) {
        lock (Continuations) {
            Continuations.AddLast(scheduledContinuation);
        }
    }

    public static void Schedule<T1>(AnabasisPlatformLoopStep timing, Action<T1> action, T1 arg) {
        Schedule(timing.ToMask(), action, arg);
    }

    public static void Schedule<T1>(AnabasisPlatformStepMask timing, Action<T1> action, T1 arg) {
        PooledDelegates.Releaser releaser = PooledDelegates.GetPooledAction(action, arg, out Action a);
        Schedule(new ScheduledContinuation(timing, a, releaser));
    }
}