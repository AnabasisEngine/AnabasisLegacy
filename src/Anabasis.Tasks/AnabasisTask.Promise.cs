namespace Anabasis.Tasks;

public readonly partial struct AnabasisTask
{
    private static AnabasisTask Create<TPromise, TArg>(TArg arg, CancellationToken cancellationToken)
        where TPromise : IPromise<TArg> => new(TPromise.Create(arg, cancellationToken, out short tok), tok);

    private interface IPromise<in TArg>
    {
        public static abstract IAnabasisTaskSource Create(TArg arg, CancellationToken cancellationToken,
            out short token);
    }

    private abstract class BasicPromise : IAnabasisTaskSource
    {
        protected ManualResetAnabasisTaskSourceCore<object> Core;

        public abstract void MoveNext();
        protected abstract void TryReturn();


        public void GetResult(short token) {
            try {
                Core.GetResult(token);
            }
            finally {
                TryReturn();
            }
        }

        public AnabasisTaskStatus GetStatus(short token) => Core.GetStatus(token);

        public AnabasisTaskStatus UnsafeGetStatus() => Core.UnsafeGetStatus();

        public void OnCompleted(Action<object?> continuation, object? state, short token) =>
            Core.OnCompleted(continuation, state, token);
    }
}