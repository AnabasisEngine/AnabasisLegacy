using System.Buffers;
using System.Reflection;
using Anabasis.Core.Handles;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Buffers;

public sealed class BufferSliceMapping<T> : MemoryManager<T>
    where T : unmanaged
{
    public readonly  int          Offset;
    public readonly  int          Length;
    private readonly BufferHandle _buffer;
    private readonly GL           _gl;

    public unsafe BufferSliceMapping(BufferHandle buffer, GL gl, int offset, int length, MapBufferAccessMask mask) {
        _buffer = buffer;
        _gl = gl;
        Offset = offset;
        Length = length;
        Pointer = (T*)_gl.MapNamedBufferRange(_buffer.Value, Offset, (nuint)Length, mask);
    }

    public unsafe T* Pointer { get; set; }

    protected override unsafe void Dispose(bool disposing) {
        _gl.UnmapNamedBuffer(_buffer.Value);
        Pointer = null;
    }

    public override unsafe Span<T> GetSpan() => new(Pointer, Length);

    public override unsafe MemoryHandle Pin(int elementIndex = 0) {
        if (elementIndex < 0 || elementIndex >= Length)
            throw new ArgumentOutOfRangeException(nameof(elementIndex));
        return new MemoryHandle(Pointer + elementIndex);
    }

    public override void Unpin() { }
}