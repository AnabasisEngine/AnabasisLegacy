using System.Buffers;
using Anabasis.Platform.Silk.Internal;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Buffers;

public sealed class BufferMappingRange<T> : MemoryManager<T>
    where T : unmanaged
{
    public readonly  int                Offset;
    public readonly  int                Length;
    private readonly BufferObjectHandle _buffer;
    private readonly IGlApi             _gl;

    public unsafe BufferMappingRange(SilkBufferObject<T> buffer, IGlApi gl, int offset, int length,
        MapBufferAccessMask mask) {
        _buffer = buffer.Handle;
        _gl = gl;
        Offset = offset;
        Length = length;
        Pointer = _gl.UnsafeMapNamedBufferRange<T>(_buffer, Offset, Length, mask);
    }

    public unsafe T* Pointer { get; set; }

    public void Flush() {
        _gl.FlushMappedNamedBufferRange(_buffer, Offset, Length);
    }

    public void Fence() {
        _gl.FenceAndWait(2000);
    }

    public void Barrier() {
        _gl.MemoryBarrier(MemoryBarrierMask.ClientMappedBufferBarrierBit);
    }

    protected override unsafe void Dispose(bool disposing) {
        _gl.UnmapNamedBuffer(_buffer);
        Pointer = null;
    }

    public override unsafe Span<T> GetSpan() => new Span<T>(Pointer, Length);

    public override unsafe MemoryHandle Pin(int elementIndex = 0) {
        if (elementIndex < 0 || elementIndex >= Length)
            throw new ArgumentOutOfRangeException(nameof(elementIndex));
        return new MemoryHandle(Pointer + elementIndex);
    }

    public override void Unpin() { }
}