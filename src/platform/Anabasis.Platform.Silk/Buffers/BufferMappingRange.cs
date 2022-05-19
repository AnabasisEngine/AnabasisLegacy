using Anabasis.Platform.Silk.Internal;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Buffers;

public ref struct BufferMappingRange<T>
    where T : unmanaged
{
    public           Span<T>            Span;
    public readonly  int                Offset;
    public readonly  int                Length;
    private readonly BufferObjectHandle _buffer;
    private readonly IGlApi             _gl;

    public BufferMappingRange(SilkBufferObject<T> buffer, IGlApi gl, Range range, MapBufferAccessMask mask) {
        _buffer = buffer.Handle;
        _gl = gl;
        (Offset, Length) = range.GetOffsetAndLength(buffer.Length);
        Span = _gl.MapNamedBufferRange<T>(_buffer, Offset, Length, mask);
    }

    public void Flush() {
        _gl.FlushMappedNamedBufferRange(_buffer, Offset, Length);
    }

    public void Fence() {
        _gl.FenceAndWait(2000);
    }

    public void Barrier() {
        _gl.MemoryBarrier(MemoryBarrierMask.ClientMappedBufferBarrierBit);
    }

    public void Dispose() {
        _gl.UnmapNamedBuffer(_buffer);
    }
}