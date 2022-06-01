namespace Anabasis.Tasks.Linq;

public static partial class AnabasisTaskAsyncEnumerable
{
    public static AnabasisTask<bool> AllAsync<TSource>(this IAnabasisTaskAsyncEnumerable<TSource> source,
        Func<TSource, bool> predicate, CancellationToken cancellationToken = default) {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return All.AllAsync(source, predicate, cancellationToken);
    }

    public static AnabasisTask<bool> AllAwaitAsync<TSource>(this IAnabasisTaskAsyncEnumerable<TSource> source,
        Func<TSource, AnabasisTask<bool>> predicate, CancellationToken cancellationToken = default) {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return All.AllAwaitAsync(source, predicate, cancellationToken);
    }

    public static AnabasisTask<bool> AllAwaitWithCancellationAsync<TSource>(
        this IAnabasisTaskAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, AnabasisTask<bool>> predicate, CancellationToken cancellationToken = default) {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return All.AllAwaitWithCancellationAsync(source, predicate, cancellationToken);
    }
}

internal static class All
{
    internal static async AnabasisTask<bool> AllAsync<TSource>(IAnabasisTaskAsyncEnumerable<TSource> source,
        Func<TSource, bool> predicate, CancellationToken cancellationToken) {
        await foreach (TSource e1 in source) {
            if (!predicate(e1))
                return false;
        }

        return true;
    }

    internal static async AnabasisTask<bool> AllAwaitAsync<TSource>(IAnabasisTaskAsyncEnumerable<TSource> source,
        Func<TSource, AnabasisTask<bool>> predicate, CancellationToken cancellationToken) {
        await foreach (TSource e1 in source) {
            if (!await predicate(e1))
                return false;
        }

        return true;
    }

    internal static async AnabasisTask<bool> AllAwaitWithCancellationAsync<TSource>(
        IAnabasisTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, AnabasisTask<bool>> predicate,
        CancellationToken cancellationToken) {
        await foreach (TSource e1 in source) {
            if (!await predicate(e1, cancellationToken))
                return false;
        }

        return true;
    }
}