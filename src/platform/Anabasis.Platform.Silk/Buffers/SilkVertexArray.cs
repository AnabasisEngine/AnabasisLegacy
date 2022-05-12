using System.Runtime.InteropServices;
using Anabasis.Platform.Abstractions;
using Anabasis.Platform.Abstractions.Buffer;
using Anabasis.Utility;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Buffers;

public class SilkVertexArray<TVertex, TIndex> : IVertexArray<TVertex, TIndex>, ISilkVertexArray<TVertex>
    where TVertex : unmanaged
    where TIndex : unmanaged
{
    public uint Handle { get; }
    private readonly GL   _gl;

    internal SilkVertexArray(GL gl) {
        _gl = gl;
        Handle = _gl.CreateVertexArray();
    }

    public void Use() {
        _gl.BindVertexArray(Handle);
    }

    public void Dispose() {
        GC.SuppressFinalize(this);
        _gl.DeleteVertexArray(Handle);
    }

    public void BindVertexBuffer(IBufferObject<TVertex> bufferObject, IBindingIndex bindingIndex) {
        uint buffer = Guard.IsType<SilkBufferObject<TVertex>>(bufferObject).Handle;
        uint idx = Guard.IsType<SilkBindingIndex>(bindingIndex).Value;
        _gl.VertexArrayVertexBuffer(Handle, idx, buffer, 0, (uint)Marshal.SizeOf<TVertex>());
    }

    public void BindIndexBuffer(IBufferObject<TIndex> bufferObject) {
        uint buffer = Guard.IsType<SilkBufferObject<TIndex>>(bufferObject).Handle;
        _gl.VertexArrayElementBuffer(Handle, buffer);
    }

    public void DrawArrays(DrawMode drawMode, int first, uint count) {
        PrimitiveType primitiveType = drawMode switch {
            DrawMode.Triangles => PrimitiveType.Triangles,
            _ => throw new ArgumentOutOfRangeException(nameof(drawMode), drawMode, null),
        };
        _gl.DrawArrays(primitiveType, first, count);
    }

    public unsafe void DrawElements(DrawMode drawMode, uint count, uint indexOffset) {
        PrimitiveType primitiveType = drawMode switch {
            DrawMode.Triangles => PrimitiveType.Triangles,
            _ => throw new ArgumentOutOfRangeException(nameof(drawMode), drawMode, null),
        };
        DrawElementsType indexType = Type.GetTypeCode(typeof(TIndex)) switch {
            TypeCode.Byte => DrawElementsType.UnsignedByte,
            TypeCode.UInt16 => DrawElementsType.UnsignedShort,
            TypeCode.UInt32 => DrawElementsType.UnsignedInt,
            _ => throw new ArgumentOutOfRangeException(nameof(TIndex)),
        };
        _gl.DrawElements(primitiveType, count, indexType, (void*)(indexOffset * sizeof(TIndex)));
    }
}