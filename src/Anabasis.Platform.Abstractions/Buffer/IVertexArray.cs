using Anabasis.Platform.Abstractions.Resources;

namespace Anabasis.Platform.Abstractions.Buffer;

public interface IVertexArray<TVertex> : IGraphicsResource
    where TVertex : unmanaged
{
    public void BindVertexBuffer(IBufferObject<TVertex> buffer, IBindingIndex bindingIndex);

    public void DrawArrays(DrawMode drawMode, int first, uint count);
}

public interface IVertexArray<TVertex, TIndex> : IVertexArray<TVertex>, IGraphicsResource
    where TVertex : unmanaged
    where TIndex : unmanaged
{
    public void BindIndexBuffer(IBufferObject<TIndex> buffer);
    public void DrawElements(DrawMode drawMode, uint count, uint indexOffset);
}