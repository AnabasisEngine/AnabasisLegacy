using Anabasis.Platform.Silk.Buffers;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

public partial interface IGlApi
{
    void BufferData<T0>(BufferTargetARB target, ReadOnlySpan<T0> data, BufferUsageARB usage)
        where T0 : unmanaged;

    void BufferSubData<T0>(BufferTargetARB target, nint offset, ReadOnlySpan<T0> data)
        where T0 : unmanaged;

    void NamedBufferData<T>(BufferObjectHandle buffer, ReadOnlySpan<T> data, BufferUsageARB usage)
        where T : unmanaged;

    void NamedBufferSubData<T>(BufferObjectHandle buffer, nint offset, ReadOnlySpan<T> data)
        where T : unmanaged;

    BufferObjectHandle CreateBuffer();
    void DeleteBuffer(BufferObjectHandle handle);
    void BindBuffer(BufferTargetARB target, BufferObjectHandle handle);

    void NamedBufferStorage<T>(BufferObjectHandle handle, nint length, ReadOnlySpan<T> data,
        BufferStorageMask staticDraw)
        where T : unmanaged;

    unsafe Span<T> MapNamedBuffer<T>(BufferObjectHandle handle, BufferAccessARB access)
        where T : unmanaged;

    unsafe Span<T> MapNamedBufferRange<T>(BufferObjectHandle handle, int offset, int length, MapBufferAccessMask mask)
        where T : unmanaged;

    void FlushMappedNamedBufferRange(BufferObjectHandle handle, int offset, int length);
    void UnmapNamedBuffer(BufferObjectHandle handle);
}