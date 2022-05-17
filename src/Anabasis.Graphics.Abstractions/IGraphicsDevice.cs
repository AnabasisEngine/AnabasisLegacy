using Anabasis.Graphics.Abstractions.Buffer;

namespace Anabasis.Graphics.Abstractions;

public interface IGraphicsDevice
{
    public IVertexArray<TVertex, TIndex> CreateVertexArray<TVertex, TIndex>()
        where TVertex : unmanaged
        where TIndex : unmanaged;

    public IBufferObject<T> CreateBuffer<T>(BufferType bufferType)
        where T : unmanaged;
}