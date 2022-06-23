using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Anabasis.Tasks;

public readonly partial struct AnabasisTask
{
    public static YieldAwaitable Yield(AnabasisPlatformLoopStep timing = AnabasisPlatformLoopStep.Update) =>
        // optimized for single continuation
        new(timing);


    public static AnabasisTask Yield(AnabasisPlatformLoopStep timing, CancellationToken cancellationToken) =>
        Create<YieldPromise, AnabasisPlatformLoopStep>(timing, cancellationToken);

    private sealed class YieldPromise : BasicPromise, IPromise<AnabasisPlatformLoopStep>
    {
        static readonly ObjectPool<YieldPromise> Pool = new(() => new YieldPromise());

        CancellationToken                         _cancellationToken;

        private YieldPromise() { }

        public static IAnabasisTaskSource Create(AnabasisPlatformLoopStep timing, CancellationToken cancellationToken,
            out short token) {
            if (cancellationToken.IsCancellationRequested) {
                return AutoResetAnabasisTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
            }

            YieldPromise result = Pool.Allocate();


            result._cancellationToken = cancellationToken;

            AnabasisTaskScheduler.Schedule(timing, result.MoveNext);

            token = result.Core.Version;
            return result;
        }

        public override void MoveNext() {
            if (_cancellationToken.IsCancellationRequested) {
                Core.TrySetCanceled(_cancellationToken);
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

public readonly struct YieldAwaitable
{
    readonly AnabasisPlatformLoopStep _timing;

    public YieldAwaitable(AnabasisPlatformLoopStep timing) {
        this._timing = timing;
    }

    public Awaiter GetAwaiter() {
        return new Awaiter(_timing);
    }

    public AnabasisTask ToAnabasisTask() {
        return AnabasisTask.Yield(_timing, CancellationToken.None);
    }

    public readonly struct Awaiter : ICriticalNotifyCompletion
    {
        readonly AnabasisPlatformLoopStep timing;

        public Awaiter(AnabasisPlatformLoopStep timing) {
            this.timing = timing;
        }

        public bool IsCompleted => false;

        public void GetResult() { }

        public void OnCompleted(Action continuation) {
            AnabasisTaskScheduler.Schedule(timing, continuation);
        }

        public void UnsafeOnCompleted(Action continuation) {
            AnabasisTaskScheduler.Schedule(timing, continuation);
        }
    }
}