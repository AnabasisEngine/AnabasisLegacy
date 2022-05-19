using System.Diagnostics;
using System.Runtime.InteropServices;
using Anabasis.Platform.Silk.Buffers;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

internal partial class GlApi
{
    public void BufferData<T0>(BufferTargetARB target, ReadOnlySpan<T0> data, BufferUsageARB usage)
        where T0 : unmanaged {
        _gl.BufferData(target, data, usage);
    }

    public void BufferSubData<T0>(BufferTargetARB target, nint offset, ReadOnlySpan<T0> data)
        where T0 : unmanaged {
        _gl.BufferSubData(target, offset, data);
    }

    public void NamedBufferData<T>(BufferObjectHandle buffer, ReadOnlySpan<T> data, BufferUsageARB usage)
        where T : unmanaged => _gl.NamedBufferData(buffer.Value, (nuint)(data.Length * Marshal.SizeOf<T>()), data,
        (VertexBufferObjectUsage)usage);

    public void NamedBufferSubData<T>(BufferObjectHandle buffer, nint offset, ReadOnlySpan<T> data)
        where T : unmanaged => _gl.NamedBufferSubData(buffer.Value, offset * Marshal.SizeOf<T>(), (nuint)(data.Length * Marshal
        .SizeOf<T>()),
        data);

    public BufferObjectHandle CreateBuffer() => new(_gl.CreateBuffer());

    public void DeleteBuffer(BufferObjectHandle handle) {
        _gl.DeleteBuffer(handle.Value);
    }

    public void BindBuffer(BufferTargetARB target, BufferObjectHandle handle) {
        _gl.BindBuffer(target, handle.Value);
    }

    public void NamedBufferStorage<T>(BufferObjectHandle handle, nint length, ReadOnlySpan<T> data,
        BufferStorageMask flags)
        where T : unmanaged {
        _gl.NamedBufferStorage(handle.Value, (nuint)(length * Marshal.SizeOf<T>()), data, flags);
    }

    public unsafe Span<T> MapNamedBuffer<T>(BufferObjectHandle handle, BufferAccessARB access)
        where T : unmanaged {
        _gl.GetNamedBufferParameter(handle.Value, BufferPNameARB.BufferSize, out int size);
        void* ptr = _gl.MapNamedBuffer(handle.Value, access);
        if (ptr == null)
            GetAndThrowError();
        return new Span<T>(ptr, size);
    }

    public unsafe Span<T> MapNamedBufferRange<T>(BufferObjectHandle handle, int offset, int length, MapBufferAccessMask mask)
        where T : unmanaged {
        void* ptr = _gl.MapNamedBufferRange(handle.Value, offset * sizeof(T), (nuint)(length * sizeof(T)), mask);
        if (ptr == null)
            GetAndThrowError();
        return new Span<T>(ptr, length);
    }

    public void FlushMappedNamedBufferRange(BufferObjectHandle handle, int offset, int length) {
        _gl.FlushMappedNamedBufferRange(handle.Value, offset, (nuint)length);
    }

    public void UnmapNamedBuffer(BufferObjectHandle handle) {
        if (!_gl.UnmapNamedBuffer(handle.Value))
            GetAndThrowError();
    }
}