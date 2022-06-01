namespace Anabasis.Tasks.Linq;

public static partial class AnabasisTaskAsyncEnumerable
{
    public static AnabasisTask<bool> AnyAsync<TSource>(this IAnabasisTaskAsyncEnumerable<TSource> source,
        CancellationToken cancellationToken = default) {
        if (source == null) throw new ArgumentNullException(nameof(source));

        return Any.AnyAsync(source, cancellationToken);
    }

    public static AnabasisTask<bool> AnyAsync<TSource>(this IAnabasisTaskAsyncEnumerable<TSource> source,
        Func<TSource, bool> predicate, CancellationToken cancellationToken = default) {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return Any.AnyAsync(source, predicate, cancellationToken);
    }

    public static AnabasisTask<bool> AnyAwaitAsync<TSource>(this IAnabasisTaskAsyncEnumerable<TSource> source,
        Func<TSource, AnabasisTask<bool>> predicate, CancellationToken cancellationToken = default) {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return Any.AnyAwaitAsync(source, predicate, cancellationToken);
    }

    public static AnabasisTask<bool> AnyAwaitWithCancellationAsync<TSource>(
        this IAnabasisTaskAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, AnabasisTask<bool>> predicate,
        CancellationToken cancellationToken = default) {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return Any.AnyAwaitWithCancellationAsync(source, predicate, cancellationToken);
    }
}

internal static class Any
{
    internal static async AnabasisTask<bool> AnyAsync<TSource>(IAnabasisTaskAsyncEnumerable<TSource> source,
        CancellationToken cancellationToken) {
        await foreach (TSource unused in source) {
            if(cancellationToken.IsCancellationRequested) break;
            return true;
        }

        return false;
    }

    internal static async AnabasisTask<bool> AnyAsync<TSource>(IAnabasisTaskAsyncEnumerable<TSource> source,
        Func<TSource, bool> predicate, CancellationToken cancellationToken) {
        await foreach (TSource e in source) {
            if(cancellationToken.IsCancellationRequested) break;
            if (predicate(e))
                return true;
        }

        return false;
    }

    internal static async AnabasisTask<bool> AnyAwaitAsync<TSource>(IAnabasisTaskAsyncEnumerable<TSource> source,
        Func<TSource, AnabasisTask<bool>> predicate, CancellationToken cancellationToken) {
        await foreach (TSource e in source) {
            if(cancellationToken.IsCancellationRequested) break;
            if (await predicate(e))
                return true;
        }

        return false;
    }

    internal static async AnabasisTask<bool> AnyAwaitWithCancellationAsync<TSource>(
        IAnabasisTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, AnabasisTask<bool>> predicate,
        CancellationToken cancellationToken) {
        await foreach (TSource e in source) {
            if (await predicate(e, cancellationToken))
                return true;
        }

        return false;
    }
}