using Anabasis.Platform.Silk.Internal;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Buffers;

public ref struct BufferMapping<T>
    where T : unmanaged
{
    public           Span<T>            Span;
    private readonly BufferObjectHandle _buffer;
    private readonly IGlApi             _gl;

    public BufferMapping(SilkBufferObject<T> buffer, IGlApi gl, BufferAccessARB access) {
        _buffer = buffer.Handle;
        _gl = gl;
        Span = _gl.MapNamedBuffer<T>(_buffer, access);
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