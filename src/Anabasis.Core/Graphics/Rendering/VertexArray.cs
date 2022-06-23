using System.Runtime.InteropServices;
using Anabasis.Core.Graphics.Buffers;
using Anabasis.Core.Graphics.Handles;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Rendering;

public readonly record struct VertexArrayBindingIndex(uint Value)
{
    private static uint _next = 0;
    public static VertexArrayBindingIndex NextIndex => new(_next++);
}

public sealed class VertexArray : AnabasisBindableGraphicsObject<VertexArrayHandle>
{
    public VertexArray(GL api) : base(api, new VertexArrayHandle(api.CreateVertexArray())) { }
    
    public DrawElementsType? IndexType { get; private set; }
    public uint IndexOffset { get; private set; }

    public void BindVertexBuffer(GraphicsBuffer buffer, VertexArrayBindingIndex bindingIndex, int offset, uint stride) {
        Api.VertexArrayVertexBuffer(Handle.Value, bindingIndex.Value, buffer.Handle.Value, offset, stride);
    }

    public void BindIndexBuffer(GraphicsBuffer buffer) {
        Api.VertexArrayElementBuffer(Handle.Value, buffer.Handle.Value);
    }

    public void BindIndexBuffer(GraphicsBuffer.TypedBufferSlice<ushort> slice) {
        BindIndexBuffer(slice.Buffer);
        IndexOffset = (uint)slice.Offset;
        IndexType = DrawElementsType.UnsignedShort;
    }
    
    public void BindIndexBuffer(GraphicsBuffer.TypedBufferSlice<byte> slice) {
        BindIndexBuffer(slice.Buffer);
        IndexOffset = (uint)slice.Offset;
        IndexType = DrawElementsType.UnsignedByte;
    }
    
    public void BindIndexBuffer(GraphicsBuffer.TypedBufferSlice<uint> slice) {
        BindIndexBuffer(slice.Buffer);
        IndexOffset = (uint)slice.Offset;
        IndexType = DrawElementsType.UnsignedInt;
    }
}

public static class VertexArrayExtensions
{
    public static VertexArrayBindingIndex FormatAndBindVertexBuffer(this VertexArray vertexArray, GraphicsBuffer buffer,
        VertexFormat vertexFormat, int offset, uint stride) {
        VertexArrayBindingIndex idx = VertexArrayBindingIndex.NextIndex;
        vertexArray.BindVertexBuffer(buffer, idx, offset, stride);
        vertexFormat(idx, new VertexFormatter(vertexArray.Api), vertexArray.Handle);
        return idx;
    }

    public static VertexArrayBindingIndex FormatAndBindVertexBuffer<T>(this VertexArray array, GraphicsBuffer buffer)
        where T : unmanaged, IVertexType => FormatAndBindVertexBuffer(array, buffer.Typed<T>());

    public static VertexArrayBindingIndex FormatAndBindVertexBuffer<T>(this VertexArray vertexArray,
        GraphicsBuffer.TypedBufferSlice<T> slice)
        where T : unmanaged, IVertexType {
        VertexArrayBindingIndex idx = VertexArrayBindingIndex.NextIndex;
        vertexArray.BindVertexBuffer(slice.Buffer, idx, slice.Offset, (uint)Marshal.SizeOf<T>());
        T.EstablishVertexFormat(idx, new VertexFormatter(vertexArray.Api), vertexArray.Handle);
        return idx;
    }
}