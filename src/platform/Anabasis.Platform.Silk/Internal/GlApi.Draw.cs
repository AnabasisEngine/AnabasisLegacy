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

    public unsafe void DrawElementsInstanced(PrimitiveType primitiveType, uint count, DrawElementsType indexType,
        long indexOffset,
        uint instanceCount) {
        Gl.DrawElementsInstanced(primitiveType, count, indexType, (void*)indexOffset, instanceCount);
    }

    public void Viewport(uint width, uint height) {
        Gl.Viewport(0, 0, width, height);
    }

    public void DrawArraysInstanced(PrimitiveType primitiveType, int first, uint count, uint instanceCount) {
        Gl.DrawArraysInstanced(primitiveType, first, count, instanceCount);
    }

    public unsafe void DrawElementsInstancedBaseVertexBaseInstance(PrimitiveType mode, uint count, DrawElementsType type,
        long indices, uint instancecount, int basevertex, uint baseinstance) {
        Gl.DrawElementsInstancedBaseVertexBaseInstance(mode, count, type, (void*)indices, instancecount, basevertex,
            baseinstance);
    }

    public unsafe void DrawElementsBaseVertex(PrimitiveType mode, uint count, DrawElementsType type, long indices,
        int basevertex) {
        Gl.DrawElementsBaseVertex(mode, count, type, (void*)indices, basevertex);
    }

    public unsafe void DrawElementsInstancedBaseVertex(PrimitiveType mode, uint count, DrawElementsType type, long indices,
        uint instancecount, int basevertex) {
        Gl.DrawElementsInstancedBaseVertex(mode, count, type, (void*)indices, instancecount, basevertex);
    }
}