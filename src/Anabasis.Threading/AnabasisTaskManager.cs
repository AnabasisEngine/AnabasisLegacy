using System.Runtime.CompilerServices;
using Anabasis.Abstractions;

namespace Anabasis.Threading;

public class AnabasisTaskManager : IDisposable
{
    internal static AnabasisTaskManager Current { get; private set; } = null!;

    private readonly struct ScheduledContinuation
    {
        private readonly  AnabasisPlatformStepMask _steps;
        internal readonly Action                   RunAction;

        public ScheduledContinuation(AnabasisPlatformStepMask steps, Action runAction) {
            _steps = steps;
            RunAction = runAction;
        }

        public bool ShouldRun(AnabasisPlatformLoopStep step) => _steps.HasStep(step);
    }

    private readonly LinkedList<ScheduledContinuation> _continuations = new();

    private readonly IDisposable[] _registrations = new IDisposable[8];

    public AnabasisTaskManager(IAnabasisRunLoop loop) {
        for (int i = 0; i < 8; i++) {
            _registrations[i] = loop.RegisterHandler((AnabasisPlatformLoopStep)i, -1, "TaskScheduler",
                new RunClosure(i, this).Run);
        }

        Current = this;
    }

    private class RunClosure
    {
        private readonly AnabasisTaskManager _manager;
        private readonly int                 _step;

        public RunClosure(int step, AnabasisTaskManager manager) {
            _step = step;
            _manager = manager;
        }

        internal void Run() => _manager.RunContinuations((AnabasisPlatformLoopStep)_step);
    }

    public bool IsOnMainThread => Environment.CurrentManagedThreadId == MainThreadId;
    public int MainThreadId { get; internal set; }

    public void ScheduleContinuation(Action continuation,
        AnabasisPlatformStepMask mask = AnabasisPlatformStepMask.All) {
        lock (_continuations) {
            _continuations.AddLast(new ScheduledContinuation(mask, continuation));
        }
    }

    public async ValueTask<T> RunOnGraphicsThread<T>(Func<T> func,
        AnabasisPlatformStepMask mask = AnabasisPlatformStepMask.All) {
        await Yield(mask);
        return func();
    }

    private void RunContinuations(AnabasisPlatformLoopStep step) {
        LinkedList<Action> drain = new();
        lock (_continuations) {
            for (LinkedListNode<ScheduledContinuation>? node = _continuations.First; node != null; node = node.Next) {
                ref ScheduledContinuation continuation = ref node.ValueRef;
                if (continuation.ShouldRun(step)) {
                    drain.AddLast(continuation.RunAction);
                    _continuations.Remove(node);
                }
            }
        }

        foreach (Action action in drain) {
            action();
        }
    }

    public YieldToMainLoopAwaitable Yield(AnabasisPlatformStepMask mask = AnabasisPlatformStepMask.All) =>
        new(mask, this);

    /// <summary>Provides the context for waiting when asynchronously switching into a game environment.</summary>
    public readonly struct YieldToMainLoopAwaitable
    {
        private readonly AnabasisPlatformStepMask _step;
        private readonly AnabasisTaskManager      _taskManager;

        public YieldToMainLoopAwaitable(AnabasisPlatformStepMask step, AnabasisTaskManager taskManager) {
            _step = step;
            _taskManager = taskManager;
        }

        /// <summary>Retrieves a <see cref="AnabasisTaskManager.YieldToMainLoopAwaitable.YieldAwaiter" /> object  for this instance of the class.</summary>
        /// <returns>The object that is used to monitor the completion of an asynchronous operation.</returns>
        public YieldAwaiter GetAwaiter() => new(_step, _taskManager);

        /// <summary>Provides an awaiter for switching into a target environment.</summary>
        public readonly struct YieldAwaiter
            : ICriticalNotifyCompletion,
                INotifyCompletion
        {
            private readonly AnabasisPlatformStepMask _step;
            private readonly AnabasisTaskManager      _anabasisTaskManager;

            public YieldAwaiter(AnabasisPlatformStepMask step, AnabasisTaskManager anabasisTaskManager) {
                _step = step;
                _anabasisTaskManager = anabasisTaskManager;
            }

            public void OnCompleted(Action continuation) {
                _anabasisTaskManager.ScheduleContinuation(continuation, _step);
            }

            public void UnsafeOnCompleted(Action continuation) {
                _anabasisTaskManager.ScheduleContinuation(continuation, _step);
            }

            public void GetResult() { }
            public bool IsCompleted => false;
        }
    }

    public void Dispose() {
        foreach (IDisposable registration in _registrations) {
            registration.Dispose();
        }
    }
}