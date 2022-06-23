using Anabasis.Core.Graphics.Handles;

namespace Anabasis.Core.Graphics.Rendering;

public interface IVertexType
{
    public static abstract void EstablishVertexFormat(VertexArrayBindingIndex bindingIndex, VertexFormatter gl,
        VertexArrayHandle vertexArray);
}