using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Anabasis.Tasks;

public readonly partial struct AnabasisTask
{
    public static YieldAwaitable Yield() =>
        // optimized for single continuation
        new(AnabasisPlatformLoopStep.Update);

    public static YieldAwaitable Yield(AnabasisPlatformLoopStep timing) =>
        // optimized for single continuation
        new(timing);
    

    public static AnabasisTask Yield(CancellationToken cancellationToken)
    {
        return new AnabasisTask(YieldPromise.Create(AnabasisPlatformLoopStep.Update, cancellationToken, out short token), token);
    }

    public static AnabasisTask Yield(AnabasisPlatformLoopStep timing, CancellationToken cancellationToken)
    {
        return new AnabasisTask(YieldPromise.Create(timing, cancellationToken, out short token), token);
    }
    
    sealed class YieldPromise : IAnabasisTaskSource
        {
            static ObjectPool<YieldPromise> pool = new(() => new YieldPromise());

            CancellationToken _cancellationToken;
            AnabasisTaskCompletionSourceCore<object> _core;

            private YieldPromise() {
            }

            public static IAnabasisTaskSource Create(AnabasisPlatformLoopStep timing, CancellationToken cancellationToken, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetAnabasisTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                YieldPromise result = pool.Allocate();


                result._cancellationToken = cancellationToken;

                AnabasisTaskScheduler.Schedule(timing, result.MoveNext);

                token = result._core.Version;
                return result;
            }

            public void GetResult(short token)
            {
                try
                {
                    _core.GetResult(token);
                }
                finally
                {
                    TryReturn();
                }
            }

            public AnabasisTaskStatus GetStatus(short token)
            {
                return _core.GetStatus(token);
            }

            public AnabasisTaskStatus UnsafeGetStatus()
            {
                return _core.UnsafeGetStatus();
            }

            public void OnCompleted(Action<object?> continuation, object? state, short token)
            {
                _core.OnCompleted(continuation, state, token);
            }

            public void MoveNext()
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    _core.TrySetCanceled(_cancellationToken);
                    return;
                }

                _core.TrySetResult(null!);
            }

            void TryReturn()
            {
                _core.Reset();
                _cancellationToken = default;
                pool.Free(this);
            }
        }
}

public readonly struct YieldAwaitable
{
    readonly AnabasisPlatformLoopStep _timing;

    public YieldAwaitable(AnabasisPlatformLoopStep timing)
    {
        this._timing = timing;
    }

    public Awaiter GetAwaiter()
    {
        return new Awaiter(_timing);
    }

    public AnabasisTask ToAnabasisTask()
    {
        return AnabasisTask.Yield(_timing, CancellationToken.None);
    }

    public readonly struct Awaiter : ICriticalNotifyCompletion
    {
        readonly AnabasisPlatformLoopStep timing;

        public Awaiter(AnabasisPlatformLoopStep timing)
        {
            this.timing = timing;
        }

        public bool IsCompleted => false;

        public void GetResult() { }

        public void OnCompleted(Action continuation)
        {
            AnabasisTaskScheduler.Schedule(timing, continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            AnabasisTaskScheduler.Schedule(timing, continuation);
        }
    }
}