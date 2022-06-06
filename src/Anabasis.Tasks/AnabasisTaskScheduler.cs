using System.Runtime.ExceptionServices;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Anabasis.Tasks;

public static partial class AnabasisTaskScheduler
{
    private readonly struct ScheduledContinuation
    {
        private readonly  AnabasisPlatformStepMask  _steps;
        internal readonly Action                    RunScheduledAction;
        internal readonly PooledDelegates.Releaser? Disposable;

        public ScheduledContinuation(AnabasisPlatformStepMask steps, Action runScheduledAction, PooledDelegates.Releaser? disposable = null) {
            _steps = steps;
            RunScheduledAction = runScheduledAction;
            Disposable = disposable;
        }

        public bool ShouldRun(AnabasisPlatformLoopStep step) => _steps.HasStep(step);
    }
    
    public static bool IsOnMainThread => Environment.CurrentManagedThreadId == MainThreadId;
    public static int MainThreadId { get; internal set; }
    
    private static readonly LinkedList<ScheduledContinuation> Continuations = new();

    internal static readonly Action<AnabasisPlatformLoopStep> RunAction = RunContinuations;
    internal static void RunContinuations(AnabasisPlatformLoopStep step) {
        LinkedList<ScheduledContinuation> drain = new();
        lock (Continuations) {
            for (LinkedListNode<ScheduledContinuation>? node = Continuations.First; node != null; node = node.Next) {
                ref ScheduledContinuation continuation = ref node.ValueRef;
                if (!continuation.ShouldRun(step)) continue;
                drain.AddLast(continuation);
                Continuations.Remove(node);
            }
        }

        foreach (ScheduledContinuation action in drain) {
            action.RunScheduledAction();
            action.Disposable?.Dispose();
        }
    }

    public static void PublishUnobservedTaskException(Exception exception) {
        Schedule(AnabasisPlatformStepMask.All, e => e.Throw(), ExceptionDispatchInfo.Capture(exception));
    }
}