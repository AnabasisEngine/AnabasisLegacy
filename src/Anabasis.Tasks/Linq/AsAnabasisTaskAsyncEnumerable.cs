namespace Anabasis.Tasks.Linq;

public static partial class AnabasisTaskAsyncEnumerable
{
    public static IAnabasisTaskAsyncEnumerable<TSource> AsAnabasisTaskAsyncEnumerable<TSource>(this IAnabasisTaskAsyncEnumerable<TSource> source) => source;
}