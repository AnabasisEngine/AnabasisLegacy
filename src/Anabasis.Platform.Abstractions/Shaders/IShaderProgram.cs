using Anabasis.Platform.Abstractions.Buffer;
using Anabasis.Platform.Abstractions.Resources;

namespace Anabasis.Platform.Abstractions.Shaders;

public interface IShaderProgram<TVertex> : IGraphicsResource
    where TVertex : unmanaged
{
    public IVertexBufferFormatter<TVertex> VertexFormatter { get; }
}