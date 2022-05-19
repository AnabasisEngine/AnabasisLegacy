using Anabasis.Platform.Silk.Buffers;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

internal partial class GlApi
{
    public void EnableVertexArrayAttrib(VertexArrayHandle handle, uint layout) {
        _gl.EnableVertexArrayAttrib(handle.Value, layout);
    }

    public void VertexArrayAttribBinding(VertexArrayHandle vaobj, uint attribindex, SilkBindingIndex bindingindex) {
        _gl.VertexArrayAttribBinding(vaobj.Value, attribindex, bindingindex.Value);
    }

    public VertexArrayHandle CreateVertexArray() => new(_gl.CreateVertexArray());

    public void BindVertexArray(VertexArrayHandle handle) {
        _gl.BindVertexArray(handle.Value);
    }

    public void DeleteVertexArray(VertexArrayHandle handle) {
        _gl.DeleteVertexArray(handle.Value);
    }

    public void VertexArrayVertexBuffer(VertexArrayHandle handle, SilkBindingIndex idx, BufferObjectHandle buffer,
        int i, uint sizeOf) {
        _gl.VertexArrayVertexBuffer(handle.Value, idx.Value, buffer.Value, i, sizeOf);
    }

    public void VertexArrayElementBuffer(VertexArrayHandle handle, BufferObjectHandle buffer) {
        _gl.VertexArrayElementBuffer(handle.Value, buffer.Value);
    }

    public void VertexArrayAttribFormat(VertexArrayHandle vaobj, uint attribindex, int size, VertexAttribType type,
        bool normalized, uint relativeoffset) {
        _gl.VertexArrayAttribFormat(vaobj.Value, attribindex, size, type, normalized, relativeoffset);
    }
}