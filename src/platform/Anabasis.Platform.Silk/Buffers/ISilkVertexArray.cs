using Anabasis.Graphics.Abstractions.Buffer;

namespace Anabasis.Platform.Silk.Buffers;

internal interface ISilkVertexArray<TVertex> : IVertexArray<TVertex>, IGlObject<VertexArrayHandle>
    where TVertex : unmanaged
{
    new VertexArrayHandle Handle { get; }
}