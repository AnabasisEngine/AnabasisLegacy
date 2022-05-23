using System.Diagnostics;
using System.Runtime.InteropServices;
using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Graphics.Abstractions.Internal;
using Anabasis.Platform.Silk.Internal;
using Anabasis.Utility;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Buffers;

public class SilkVertexArray : SilkGlObject<VertexArrayHandle>, IVertexArray
{
    internal SilkVertexArray(IGlApi gl) : base(gl, gl.CreateVertexArray()) {
    }

    private int?              _indexSize;
    private DrawElementsType? _indexType;

    public override IDisposable Use() {
        Gl.BindVertexArray(Handle);
        return new GenericDisposer(() => Gl.BindVertexArray(new VertexArrayHandle(0)));
    }

    public override void Dispose() {
        GC.SuppressFinalize(this);
        Gl.DeleteVertexArray(Handle);
    }

    public void BindVertexBuffer<TVertex>(IBufferObject<TVertex> bufferObject, IBindingIndex bindingIndex)
        where TVertex : unmanaged {
        BufferObjectHandle buffer = Guard.IsType<SilkBufferObject<TVertex>>(bufferObject).Handle;
        SilkBindingIndex idx = Guard.IsType<SilkBindingIndex>(bindingIndex);
        Gl.VertexArrayVertexBuffer(Handle, idx, buffer, 0, (uint)Marshal.SizeOf<TVertex>());
    }

    public void BindIndexBuffer<TIndex>(IBufferObject<TIndex> bufferObject)
        where TIndex : unmanaged {
        BufferObjectHandle buffer = Guard.IsType<SilkBufferObject<TIndex>>(bufferObject).Handle;
        Gl.VertexArrayElementBuffer(Handle, buffer);
        _indexSize = Marshal.SizeOf<TIndex>();
        _indexType = Type.GetTypeCode(typeof(TIndex)) switch {
            TypeCode.Byte => DrawElementsType.UnsignedByte,
            TypeCode.UInt16 => DrawElementsType.UnsignedShort,
            TypeCode.UInt32 => DrawElementsType.UnsignedInt,
            _ => throw new ArgumentOutOfRangeException(nameof(TIndex)),
        };
    }

    public void DrawArrays(DrawMode drawMode, int first, uint count) {
        PrimitiveType primitiveType = drawMode switch {
            DrawMode.Triangles => PrimitiveType.Triangles,
            _ => throw new ArgumentOutOfRangeException(nameof(drawMode), drawMode, null),
        };
        Gl.DrawArrays(primitiveType, first, count);
    }

    public void DrawArraysInstanced(DrawMode drawMode, int first, uint count, uint instances) {
        PrimitiveType primitiveType = drawMode switch {
            DrawMode.Triangles => PrimitiveType.Triangles,
            _ => throw new ArgumentOutOfRangeException(nameof(drawMode), drawMode, null),
        };
        Gl.DrawArraysInstanced(primitiveType, first, count, instances);
        Gl.GetAndThrowError();
    }

    public unsafe void DrawElements(DrawMode drawMode, uint count, uint indexOffset) {
        PrimitiveType primitiveType = drawMode switch {
            DrawMode.Triangles => PrimitiveType.Triangles,
            _ => throw new ArgumentOutOfRangeException(nameof(drawMode), drawMode, null),
        };
        Debug.Assert(_indexSize != null, nameof(_indexSize) + " != null");
        Debug.Assert(_indexType != null, nameof(_indexType) + " != null");
        Gl.DrawElements(primitiveType, count, _indexType.Value, indexOffset * _indexSize.Value);
    }

    public void DrawElementsInstanced(DrawMode drawMode, uint count, uint indexOffset, uint instanceCount) {
        PrimitiveType primitiveType = drawMode switch {
            DrawMode.Triangles => PrimitiveType.Triangles,
            _ => throw new ArgumentOutOfRangeException(nameof(drawMode), drawMode, null),
        };
        Debug.Assert(_indexSize != null, nameof(_indexSize) + " != null");
        Debug.Assert(_indexType != null, nameof(_indexType) + " != null");
        Gl.DrawElementsInstanced(primitiveType, count, _indexType.Value, indexOffset * _indexSize.Value, instanceCount);
    }
}