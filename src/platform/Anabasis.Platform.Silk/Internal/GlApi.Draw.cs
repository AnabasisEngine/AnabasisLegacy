using Anabasis.Graphics.Abstractions;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

internal partial class GlApi
{
    public void DrawArrays(PrimitiveType primitiveType, int first, uint count) {
        Gl.DrawArrays(primitiveType, first, count);
    }

    public unsafe void DrawElements(PrimitiveType primitiveType, uint count, DrawElementsType indexType,
        long indexOffset) {
        Gl.DrawElements(primitiveType, count, indexType, (void*)indexOffset);
    }

    public void ClearColor(Color color) {
        color.Deconstruct(out float r, out float g, out float b, out float a);
        Gl.ClearColor(r, g, b, a);
    }

    public void ClearFlags(ClearBufferMask mask) {
        Gl.Clear(mask);
    }

    public unsafe void DrawElementsInstanced(PrimitiveType primitiveType, uint count, DrawElementsType indexType, long indexOffset,
        uint instanceCount) {
        Gl.DrawElementsInstanced(primitiveType, count, indexType, (void*)indexOffset, instanceCount);
    }

    public void Viewport(uint width, uint height) {
        Gl.Viewport(0, 0, width, height);
    }

    public void DrawArraysInstanced(PrimitiveType primitiveType, int first, uint count, uint instanceCount) {
        Gl.DrawArraysInstanced(primitiveType, first, count, instanceCount);
    }
}