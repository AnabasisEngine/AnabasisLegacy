namespace Anabasis.Graphics.Abstractions.Buffer;

public interface IVertexBufferFormatter
{
    IBindingIndex? BindingIndex { get; }
}

public interface IVertexBufferFormatter<TVertex>
    : IVertexBufferFormatter
    where TVertex : unmanaged
{
    public IBindingIndex BindVertexFormat(IVertexArray array, IBufferObject<TVertex> vertexBuffer);
}