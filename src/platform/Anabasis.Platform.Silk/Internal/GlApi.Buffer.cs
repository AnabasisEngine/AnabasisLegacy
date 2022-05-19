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
        where T : unmanaged => _gl.NamedBufferSubData(buffer.Value, offset, (nuint)(data.Length * Marshal.SizeOf<T>()),
        data);

    public BufferObjectHandle CreateBuffer() => new(_gl.CreateBuffer());

    public void DeleteBuffer(BufferObjectHandle handle) {
        _gl.DeleteBuffer(handle.Value);
    }

    public void BindBuffer(BufferTargetARB target, BufferObjectHandle handle) {
        _gl.BindBuffer(target, handle.Value);
    }
}