using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Platform.Silk.Internal;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Buffers;

public class SilkBufferObject<T> : SilkGlObject<BufferObjectHandle>, IBufferObject<T>
    where T : unmanaged
{
    private readonly BufferTargetARB _target;
    private readonly IGlApi              _gl;
    private          int             _length;
    

    internal SilkBufferObject(IGlApi gl, BufferTargetARB target) : base(gl, gl.CreateBuffer()) {
        _target = target;
        _gl = gl;
    }

    public void LoadData(ReadOnlySpan<T> data, int offset = 0) {
        if (_length >= offset + data.Length) {
            _gl.NamedBufferSubData(Handle, offset, data);
        } else {
            _gl.NamedBufferData(Handle, data, BufferUsageARB.StaticDraw);
            _length = data.Length;
        }
    }

    public override void Dispose() {
        GC.SuppressFinalize(this);
        _gl.DeleteBuffer(Handle);
    }

    public override void Use() {
        _gl.BindBuffer(_target, Handle);
    }
}