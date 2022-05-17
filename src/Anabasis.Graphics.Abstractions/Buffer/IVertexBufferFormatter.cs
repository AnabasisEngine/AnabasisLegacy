namespace Anabasis.Graphics.Abstractions.Buffer;

public interface IVertexBufferFormatter<TVertex>
    where TVertex : unmanaged
{
    public IBindingIndex? BindingIndex { get; }
    public IBindingIndex BindVertexFormat(IVertexArray<TVertex> array, IBufferObject<TVertex> vertexBuffer);
}