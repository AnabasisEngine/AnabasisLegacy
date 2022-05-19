using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

internal partial class GlApi
{
    public void DrawArrays(PrimitiveType primitiveType, int first, uint count) {
        _gl.DrawArrays(primitiveType, first, count);
    }

    public unsafe void DrawElements(PrimitiveType primitiveType, uint count, DrawElementsType indexType,
        long indexOffset) {
        _gl.DrawElements(primitiveType, count, indexType, (void*)indexOffset);
    }
}