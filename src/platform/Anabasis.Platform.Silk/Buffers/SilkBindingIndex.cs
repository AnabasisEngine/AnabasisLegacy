using System.Diagnostics.CodeAnalysis;
using Anabasis.Platform.Abstractions.Buffer;
using ValueOf;

namespace Anabasis.Platform.Silk.Buffers;

internal class SilkBindingIndex : ValueOf<uint, SilkBindingIndex>, IBindingIndex
{
    [return: NotNullIfNotNull("i")]
    public static SilkBindingIndex? FromNullable(uint? i) => i is { } idx ? From(idx) : null;
}