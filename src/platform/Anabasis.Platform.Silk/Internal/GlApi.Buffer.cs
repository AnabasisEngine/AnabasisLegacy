using System.Runtime.InteropServices;
using Anabasis.Platform.Silk.Buffers;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

internal partial class GlApi
{
    public void BufferData<T0>(BufferTargetARB target, ReadOnlySpan<T0> data, BufferUsageARB usage)
        where T0 : unmanaged {
        Gl.BufferData(target, data, usage);
    }

    public void BufferSubData<T0>(BufferTargetARB target, nint offset, ReadOnlySpan<T0> data)
        where T0 : unmanaged {
        Gl.BufferSubData(target, offset, data);
    }

    public void NamedBufferData<T>(BufferObjectHandle buffer, ReadOnlySpan<T> data, BufferUsageARB usage)
        where T : unmanaged => Gl.NamedBufferData(buffer.Value, (nuint)(data.Length * Marshal.SizeOf<T>()), data,
        (VertexBufferObjectUsage)usage);

    public void NamedBufferSubData<T>(BufferObjectHandle buffer, nint offset, ReadOnlySpan<T> data)
        where T : unmanaged => Gl.NamedBufferSubData(buffer.Value, offset * Marshal.SizeOf<T>(), (nuint)(data.Length * Marshal
        .SizeOf<T>()),
        data);

    public BufferObjectHandle CreateBuffer() => new(Gl.CreateBuffer());

    public void DeleteBuffer(BufferObjectHandle handle) {
        Gl.DeleteBuffer(handle.Value);
    }

    public void BindBuffer(BufferTargetARB target, BufferObjectHandle handle) {
        Gl.BindBuffer(target, handle.Value);
    }

    public void NamedBufferStorage<T>(BufferObjectHandle handle, nint length, ReadOnlySpan<T> data,
        BufferStorageMask flags)
        where T : unmanaged {
        Gl.NamedBufferStorage(handle.Value, (nuint)(length * Marshal.SizeOf<T>()), data, flags);
    }

    public unsafe Span<T> MapNamedBuffer<T>(BufferObjectHandle handle, BufferAccessARB access)
        where T : unmanaged {
        Gl.GetNamedBufferParameter(handle.Value, BufferPNameARB.BufferSize, out int size);
        void* ptr = Gl.MapNamedBuffer(handle.Value, access);
        if (ptr == null)
            GetAndThrowError();
        return new Span<T>(ptr, size);
    }

    public unsafe Span<T> MapNamedBufferRange<T>(BufferObjectHandle handle, int offset, int length, MapBufferAccessMask mask)
        where T : unmanaged {
        void* ptr = Gl.MapNamedBufferRange(handle.Value, offset * sizeof(T), (nuint)(length * sizeof(T)), mask);
        if (ptr == null)
            GetAndThrowError();
        return new Span<T>(ptr, length);
    }

    public void FlushMappedNamedBufferRange(BufferObjectHandle handle, int offset, int length) {
        Gl.FlushMappedNamedBufferRange(handle.Value, offset, (nuint)length);
    }

    public void UnmapNamedBuffer(BufferObjectHandle handle) {
        if (!Gl.UnmapNamedBuffer(handle.Value))
            GetAndThrowError();
    }
}