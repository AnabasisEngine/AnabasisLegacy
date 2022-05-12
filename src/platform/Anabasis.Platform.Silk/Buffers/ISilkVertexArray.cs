using Anabasis.Platform.Abstractions.Buffer;

namespace Anabasis.Platform.Silk.Buffers;

internal interface ISilkVertexArray<TVertex> : IVertexArray<TVertex>
    where TVertex : unmanaged
{
    uint Handle { get; }
}