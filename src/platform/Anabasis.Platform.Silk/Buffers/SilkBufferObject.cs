using Anabasis.Platform.Abstractions;
using Anabasis.Platform.Abstractions.Buffer;
using Anabasis.Platform.Silk.Internal;
using Anabasis.Utility;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Buffers;

public class SilkBufferObject<T> : IBufferObject<T>
    where T : unmanaged
{
    private readonly BufferTargetARB _target;
    private readonly GL              _gl;
    private          int             _length = 0;
    internal uint Handle { get; }
    

    internal SilkBufferObject(GL gl, BufferTargetARB target) {
        _target = target;
        _gl = gl;
        Handle = _gl.CreateBuffer();
    }

    public void LoadData(ReadOnlySpan<T> data, int offset = 0) {
        if (_length > offset + data.Length) {
            _gl.NamedBufferSubData(Handle, offset, data);
        } else {
            _gl.NamedBufferData(Handle, data, BufferUsageARB.StaticDraw);
        }
    }

    public void Dispose() {
        GC.SuppressFinalize(this);
        _gl.DeleteBuffer(Handle);
    }

    public void Use() {
        _gl.BindBuffer(_target, Handle);
    }
}