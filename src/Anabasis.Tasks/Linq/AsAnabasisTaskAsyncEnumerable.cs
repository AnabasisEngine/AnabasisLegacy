using System.Diagnostics.CodeAnalysis;

namespace Anabasis.Tasks.Linq;

[SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters")]
public static partial class AnabasisTaskAsyncEnumerable
{
    public static IAnabasisTaskAsyncEnumerable<TSource> AsAnabasisTaskAsyncEnumerable<TSource>(this IAnabasisTaskAsyncEnumerable<TSource> source) => source;
}