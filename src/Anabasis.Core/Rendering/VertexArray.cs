using System.Runtime.InteropServices;
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

    public void BindVertexBuffer(GraphicsBuffer buffer, VertexArrayBindingIndex bindingIndex, int offset, uint stride) {
        Gl.VertexArrayVertexBuffer(Handle.Value, bindingIndex.Value, buffer.Handle.Value, offset, stride);
    }

    public void BindIndexBuffer(GraphicsBuffer buffer) {
        Gl.VertexArrayElementBuffer(Handle.Value, buffer.Handle.Value);
    }
}

public static class VertexArrayExtensions
{
    public static VertexArrayBindingIndex FormatAndBindVertexBuffer(this VertexArray vertexArray, GraphicsBuffer buffer,
        VertexFormat vertexFormat, int offset, uint stride) {
        VertexArrayBindingIndex idx = VertexArrayBindingIndex.NextIndex;
        vertexArray.BindVertexBuffer(buffer, idx, offset, stride);
        vertexFormat(idx, new VertexFormatter(vertexArray.Gl), vertexArray.Handle);
        return idx;
    }

    public static VertexArrayBindingIndex FormatAndBindVertexBuffer<T>(this VertexArray array, GraphicsBuffer buffer)
        where T : unmanaged, IVertexType => FormatAndBindVertexBuffer(array, buffer.Typed<T>());

    public static VertexArrayBindingIndex FormatAndBindVertexBuffer<T>(this VertexArray vertexArray,
        GraphicsBuffer.TypedBufferSlice<T> slice)
        where T : unmanaged, IVertexType {
        VertexArrayBindingIndex idx = VertexArrayBindingIndex.NextIndex;
        vertexArray.BindVertexBuffer(slice.Buffer, idx, slice.Offset, (uint)Marshal.SizeOf<T>());
        T.EstablishVertexFormat(idx, new VertexFormatter(vertexArray.Gl), vertexArray.Handle);
        return idx;
    }
}