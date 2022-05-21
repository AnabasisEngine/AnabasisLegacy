using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Platform.Abstractions;

namespace Anabasis.Graphics.Abstractions;

public interface IGraphicsDevice
{
    public IVertexArray<TIndex> CreateVertexArray<TIndex>()
        where TIndex : unmanaged;

    public IBufferObject<T> CreateBuffer<T>(BufferType bufferType)
        where T : unmanaged;

    void Clear(Color color, ClearFlags flags = ClearFlags.None);
}

[Flags]
public enum ClearFlags
{
    None     = 0,
    Color    = 1,
    Stencil  = 2,
    Depth    = 4,
    Coverage = 8
}