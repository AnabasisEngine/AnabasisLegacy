using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Platform.Abstractions.Resources;

namespace Anabasis.Graphics.Abstractions.Shaders;

public interface IShaderProgram<TVertex> : IPlatformResource
    where TVertex : unmanaged
{
    public IVertexBufferFormatter<TVertex> VertexFormatter { get; }
}