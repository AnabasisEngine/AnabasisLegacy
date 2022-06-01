using System.Runtime.InteropServices;

namespace Anabasis.Tasks;

public interface IAnabasisTaskAsyncEnumerable<out T>
{
    IAnabasisTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default);
}

public interface IAnabasisTaskAsyncEnumerator<out T> : IAnabasisTaskAsyncDisposable
{
    T Current { get; }
    AnabasisTask<bool> MoveNextAsync();
}

public interface IAnabasisTaskAsyncDisposable
{
    AnabasisTask DisposeAsync();
}
public static class AnabasisTaskAsyncEnumerableExtensions
{
    public static AnabasisTaskCancelableAsyncEnumerable<T> WithCancellation<T>(this IAnabasisTaskAsyncEnumerable<T> source, CancellationToken cancellationToken) => new(source, cancellationToken);
}

[StructLayout(LayoutKind.Auto)]
public readonly struct AnabasisTaskCancelableAsyncEnumerable<T>
{
    private readonly IAnabasisTaskAsyncEnumerable<T> enumerable;
    private readonly CancellationToken          cancellationToken;

    internal AnabasisTaskCancelableAsyncEnumerable(IAnabasisTaskAsyncEnumerable<T> enumerable, CancellationToken cancellationToken)
    {
        this.enumerable = enumerable;
        this.cancellationToken = cancellationToken;
    }

    public Enumerator GetAsyncEnumerator() => new(enumerable.GetAsyncEnumerator(cancellationToken));

    [StructLayout(LayoutKind.Auto)]
    public readonly struct Enumerator
    {
        private readonly IAnabasisTaskAsyncEnumerator<T> enumerator;

        internal Enumerator(IAnabasisTaskAsyncEnumerator<T> enumerator)
        {
            this.enumerator = enumerator;
        }

        public T Current => enumerator.Current;

        public AnabasisTask<bool> MoveNextAsync()
        {
            return enumerator.MoveNextAsync();
        }


        public AnabasisTask DisposeAsync()
        {
            return enumerator.DisposeAsync();
        }
    }
}