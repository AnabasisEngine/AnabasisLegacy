using Anabasis.Core.Handles;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Rendering;

public sealed class VertexFormatter
{
    private readonly GL _gl;

    public VertexFormatter(GL gl) {
        _gl = gl;
    }

    public void WriteVertexArrayAttribFormat(ProgramHandle program, VertexArrayHandle vertexArray,
        VertexArrayBindingIndex bindingIndex, string attribName, ref int? attribIndex, int size, VertexAttribType type,
        bool normalize, int relativeOffset) {
        WriteVertexArrayAttribFormat(vertexArray, bindingIndex,
            attribIndex ??= _gl.GetAttribLocation(program.Value, attribName), size, type, normalize, relativeOffset);
    }

    public void WriteVertexArrayAttribFormat(VertexArrayHandle handle, VertexArrayBindingIndex index,
        int attribIndex, int size, VertexAttribType type, bool normalize, int relativeOffset) {
        _gl.EnableVertexArrayAttrib(handle.Value, (uint)attribIndex);
        _gl.VertexArrayAttribFormat(handle.Value, (uint)attribIndex, size, type, normalize, (uint)relativeOffset);
        _gl.VertexArrayAttribBinding(handle.Value, (uint)attribIndex, index.Value);
    }

    public void WriteVertexArrayBindingDivisor(VertexArrayHandle handle, VertexArrayBindingIndex index, uint divisor) {
        _gl.VertexArrayBindingDivisor(handle.Value, index.Value, divisor);
    }
}