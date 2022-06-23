using Microsoft.CodeAnalysis.PooledObjects;

namespace Anabasis.Tasks;

public readonly partial struct AnabasisTask
{
    internal static ulong    Frame;
    internal static TimeSpan TotalElapsed;

    public static AnabasisTask WaitForNextFrame(AnabasisPlatformLoopStep timing = AnabasisPlatformLoopStep.Update,
        CancellationToken cancellationToken = default) =>
        Create<NextFramePromise, AnabasisPlatformLoopStep>(timing, cancellationToken);

    public static AnabasisTask Delay(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        Create<DelayPromise, TimeSpan>(timeout, cancellationToken);

    private sealed class NextFramePromise : BasicPromise, IPromise<AnabasisPlatformLoopStep>
    {
        static readonly ObjectPool<NextFramePromise> Pool = new(() => new NextFramePromise());
        CancellationToken                            _cancellationToken;
        ulong                                        _currentFrame;
        private AnabasisPlatformLoopStep             _timing;

        private NextFramePromise() { }

        public static IAnabasisTaskSource Create(AnabasisPlatformLoopStep timing, CancellationToken cancellationToken,
            out short token) {
            if (cancellationToken.IsCancellationRequested) {
                return AutoResetAnabasisTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
            }

            NextFramePromise result = Pool.Allocate();
            result._cancellationToken = cancellationToken;
            result._currentFrame = Frame;
            result._timing = timing;

            AnabasisTaskScheduler.Schedule(timing, result.MoveNext);

            token = result.Core.Version;
            return result;
        }

        public override void MoveNext() {
            if (_cancellationToken.IsCancellationRequested) {
                Core.TrySetCanceled(_cancellationToken);
                return;
            }

            if (_currentFrame == Frame) {
                AnabasisTaskScheduler.Schedule(_timing, MoveNext);
                return;
            }

            Core.TrySetResult(null!);
        }

        protected override void TryReturn() {
            Core.Reset();
            _cancellationToken = default;
            Pool.Free(this);
        }
    }

    private sealed class DelayPromise : BasicPromise, IPromise<TimeSpan>
    {
        static readonly ObjectPool<DelayPromise> Pool = new(() => new DelayPromise());
        CancellationToken                        _cancellationToken;
        TimeSpan                                 _target;

        private DelayPromise() { }

        public static IAnabasisTaskSource Create(TimeSpan timeout, CancellationToken cancellationToken,
            out short token) {
            if (cancellationToken.IsCancellationRequested) {
                return AutoResetAnabasisTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
            }

            DelayPromise result = Pool.Allocate();
            result._cancellationToken = cancellationToken;
            result._target = TotalElapsed + timeout;

            AnabasisTaskScheduler.Schedule(AnabasisPlatformLoopStep.Update, result.MoveNext);

            token = result.Core.Version;
            return result;
        }

        public override void MoveNext() {
            if (_cancellationToken.IsCancellationRequested) {
                Core.TrySetCanceled(_cancellationToken);
                return;
            }

            if (TotalElapsed < _target) {
                AnabasisTaskScheduler.Schedule(AnabasisPlatformLoopStep.Update, MoveNext);
                return;
            }

            Core.TrySetResult(null!);
        }

        protected override void TryReturn() {
            Core.Reset();
            _cancellationToken = default;
            Pool.Free(this);
        }
    }
}