using Microsoft.CodeAnalysis.PooledObjects;

namespace Anabasis.Tasks;

public static partial class AnabasisTaskHelper
{
    private readonly struct ScheduledContinuation
    {
        private readonly  AnabasisPlatformStepMask  _steps;
        internal readonly Action                    RunAction;
        internal readonly PooledDelegates.Releaser? Disposable;

        public ScheduledContinuation(AnabasisPlatformStepMask steps, Action runAction, PooledDelegates.Releaser? disposable = null) {
            _steps = steps;
            RunAction = runAction;
            Disposable = disposable;
        }

        public bool ShouldRun(AnabasisPlatformLoopStep step) => _steps.HasStep(step);
    }
    
    public static bool IsOnMainThread => Environment.CurrentManagedThreadId == MainThreadId;
    public static int MainThreadId { get; internal set; }
    
    private static readonly LinkedList<ScheduledContinuation> Continuations = new();

    private static void RunContinuations(AnabasisPlatformLoopStep step) {
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
            action.RunAction();
            action.Disposable?.Dispose();
        }
    }
}