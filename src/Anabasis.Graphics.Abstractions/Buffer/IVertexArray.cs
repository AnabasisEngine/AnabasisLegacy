using Anabasis.Platform.Abstractions.Resources;

namespace Anabasis.Graphics.Abstractions.Buffer;

public interface IVertexArray : IPlatformResource
{
    public void BindVertexBuffer<TVertex>(IBufferObject<TVertex> buffer, IBindingIndex bindingIndex)
        where TVertex : unmanaged;

    public void DrawArrays(DrawMode drawMode, int first, uint count);

    public void DrawArraysInstanced(DrawMode drawMode, int first, uint count, uint instances);
}

public interface IVertexArray<TIndex> : IVertexArray, IPlatformResource
    where TIndex : unmanaged
{
    public void BindIndexBuffer(IBufferObject<TIndex> buffer);
    public void DrawElements(DrawMode drawMode, uint count, uint indexOffset);
}