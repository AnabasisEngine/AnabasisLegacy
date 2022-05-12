using Anabasis.Platform.Abstractions.Buffer;

namespace Anabasis.Platform.Abstractions;

public interface IGraphicsDevice
{
    public IVertexArray<TVertex, TIndex> CreateVertexArray<TVertex, TIndex>()
        where TVertex : unmanaged
        where TIndex : unmanaged;

    public IBufferObject<T> CreateBuffer<T>(BufferType bufferType)
        where T : unmanaged;
}