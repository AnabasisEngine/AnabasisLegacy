using Anabasis.Core.Buffers;
using Anabasis.Core.Handles;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Rendering;

public readonly record struct VertexArrayBindingIndex(uint Value)
{
    private static uint _next = 0;
    public static VertexArrayBindingIndex NextIndex => new(_next++);
}

public sealed class VertexArray : AnabasisBindableNativeObject<VertexArrayHandle>
{
    public VertexArray(GL gl) : base(gl, new VertexArrayHandle(gl.CreateVertexArray())) { }

    public void BindVertexBuffer(BufferObject buffer, VertexArrayBindingIndex bindingIndex, int offset, uint stride) {
        Gl.VertexArrayVertexBuffer(Handle.Value, bindingIndex.Value, buffer.Handle.Value, offset, stride);
    }

    public void BindIndexBuffer(BufferObject buffer) {
        Gl.VertexArrayElementBuffer(Handle.Value, buffer.Handle.Value);
    }
}

public static class VertexArrayExtensions
{
    public static VertexArrayBindingIndex FormatAndBindVertexBuffer(this VertexArray vertexArray, BufferObject buffer,
        IVertexFormat vertexFormat, int offset, uint stride) {
        VertexArrayBindingIndex idx = VertexArrayBindingIndex.NextIndex;
        vertexArray.BindVertexBuffer(buffer, idx, offset, stride);
        vertexFormat.EstablishFormat(idx, vertexArray);
        return idx;
    }
}