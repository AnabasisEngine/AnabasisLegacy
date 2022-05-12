namespace Anabasis.Platform.Abstractions.Buffer;

public interface IVertexBufferFormatter<TVertex>
    where TVertex : unmanaged
{
    public IBindingIndex? BindingIndex { get; }
    public void BindVertexFormat(IVertexArray<TVertex> array, IBufferObject<TVertex> vertexBuffer);
}