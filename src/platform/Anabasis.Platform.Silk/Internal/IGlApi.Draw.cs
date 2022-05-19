using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

public partial interface IGlApi
{
    void DrawArrays(PrimitiveType primitiveType, int first, uint count);
    void DrawElements(PrimitiveType primitiveType, uint count, DrawElementsType indexType, long indexOffset);
}