using Anabasis.Platform.Silk.Buffers;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

public partial interface IGlApi
{
    void EnableVertexArrayAttrib(VertexArrayHandle handle, uint layout);

    void VertexArrayAttribFormat(VertexArrayHandle handle, uint layout, int count, VertexAttribType pointerType, bool b,
        uint offset);

    void VertexArrayAttribBinding(VertexArrayHandle handle, uint layout, SilkBindingIndex bindingIndex);
    VertexArrayHandle CreateVertexArray();
    void BindVertexArray(VertexArrayHandle handle);
    void DeleteVertexArray(VertexArrayHandle handle);

    void VertexArrayVertexBuffer(VertexArrayHandle handle, SilkBindingIndex idx, BufferObjectHandle buffer, int i,
        uint sizeOf);

    void VertexArrayElementBuffer(VertexArrayHandle handle, BufferObjectHandle buffer);
}