using Anabasis.Platform.Abstractions.Resources;

namespace Anabasis.Graphics.Abstractions.Buffer;

public interface IVertexArray : IPlatformResource
{
    public void BindVertexBuffer<TVertex>(IBufferObject<TVertex> buffer, IBindingIndex bindingIndex)
        where TVertex : unmanaged;

    public void DrawArrays(DrawMode drawMode, int first, uint count);

    public void DrawArraysInstanced(DrawMode drawMode, int first, uint count, uint instances);

    public void BindIndexBuffer<TIndex>(IBufferObject<TIndex> buffer)
        where TIndex : unmanaged;

    public void DrawElements(DrawMode drawMode, uint count, uint indexOffset);

    public void DrawElementsInstanced(DrawMode drawMode, uint count, uint indexOffset, uint instanceCount);
    void DrawElementsBaseVertex(DrawMode drawMode, uint count, uint indexOffset, int baseVertex);

    void DrawElementsInstancedBaseVertex(DrawMode drawMode, uint count, uint indexOffset, uint instanceCount,
        int baseVertex);

    void DrawElementsInstancedBaseVertexBaseInstance(DrawMode drawMode, uint count, uint indexOffset,
        uint instanceCount, int baseVertex, uint baseInstance);
}