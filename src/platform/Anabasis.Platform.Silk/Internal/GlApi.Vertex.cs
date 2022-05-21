using Anabasis.Platform.Silk.Buffers;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

internal partial class GlApi
{
    public void EnableVertexArrayAttrib(VertexArrayHandle handle, uint layout) {
        Gl.EnableVertexArrayAttrib(handle.Value, layout);
    }

    public void VertexArrayAttribBinding(VertexArrayHandle vaobj, uint attribindex, SilkBindingIndex bindingindex) {
        Gl.VertexArrayAttribBinding(vaobj.Value, attribindex, bindingindex.Value);
    }

    public VertexArrayHandle CreateVertexArray() => new(Gl.CreateVertexArray());

    public void BindVertexArray(VertexArrayHandle handle) {
        Gl.BindVertexArray(handle.Value);
    }

    public void DeleteVertexArray(VertexArrayHandle handle) {
        Gl.DeleteVertexArray(handle.Value);
    }

    public void VertexArrayVertexBuffer(VertexArrayHandle handle, SilkBindingIndex idx, BufferObjectHandle buffer,
        int i, uint sizeOf) {
        Gl.VertexArrayVertexBuffer(handle.Value, idx.Value, buffer.Value, i, sizeOf);
    }

    public void VertexArrayElementBuffer(VertexArrayHandle handle, BufferObjectHandle buffer) {
        Gl.VertexArrayElementBuffer(handle.Value, buffer.Value);
    }

    public void VertexArrayAttribFormat(VertexArrayHandle vaobj, uint attribindex, int size, VertexAttribType type,
        bool normalized, uint relativeoffset) {
        Gl.VertexArrayAttribFormat(vaobj.Value, attribindex, size, type, normalized, relativeoffset);
    }

    public void VertexArrayBindingDivisor(VertexArrayHandle vaobj, SilkBindingIndex bindingindex, uint divisor) {
        Gl.VertexArrayBindingDivisor(vaobj.Value, bindingindex.Value, divisor);
    }
}