using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Graphics.Abstractions.Internal;
using Anabasis.Platform.Silk.Internal;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Buffers;

public class SilkBufferObject<T> : SilkGlObject<BufferObjectHandle>, IBufferObject<T>, IMappableBufferObject<T>
    where T : unmanaged
{
    private readonly BufferTargetARB _target;
    private readonly IGlApi          _gl;
    public int Length { get; private set; } = -1;
    
    internal SilkBufferObject(IGlApi gl, BufferTargetARB target) : base(gl, gl.CreateBuffer()) {
        _target = target;
        _gl = gl;
    }

    internal SilkBufferObject(IGlApi gl, BufferTargetARB target, int length, BufferAccess flags) : this(gl, target) {
        Allocate(length, ReadOnlySpan<T>.Empty, flags);
    }

    [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
    public void Allocate(int length, ReadOnlySpan<T> data = default, BufferAccess flags = BufferAccess.None) {
        Length = length;
        BufferStorageMask mask = 0;
        if (!data.IsEmpty)
            mask |= BufferStorageMask.DynamicStorageBit;

        if ((flags & BufferAccess.Persistent) != 0)
            mask |= BufferStorageMask.MapPersistentBit;
        if ((flags & BufferAccess.Coherent) != 0)
            mask |= BufferStorageMask.MapCoherentBit;
        if ((flags & BufferAccess.Read) != 0)
            mask |= BufferStorageMask.MapReadBit;
        if ((flags & BufferAccess.Write) != 0)
            mask |= BufferStorageMask.MapWriteBit;
        if ((flags & BufferAccess.Dynamic) != 0)
            mask |= BufferStorageMask.DynamicStorageBit;

        _gl.NamedBufferStorage(Handle, length, data, mask);
        _gl.GetAndThrowError();
    }

    public void LoadData(ReadOnlySpan<T> data, int offset = 0) {
        if (data.IsEmpty)
            throw new ArgumentException("Cannot load a buffer with no data");
        if (Length < 0) {
            Allocate(data.Length, data);
        } else if (Length >= offset + data.Length) {
            _gl.NamedBufferSubData(Handle, offset, data);
        } else {
            throw new InvalidOperationException(
                "Cannot change the size of a buffer after first allocation, use Allocate() to allocate sufficient storage");
        }
    }

    public override void Dispose() {
        GC.SuppressFinalize(this);
        _gl.DeleteBuffer(Handle);
    }

    public override IDisposable Use() {
        _gl.BindBuffer(_target, Handle);
        return new GenericDisposer(() => _gl.BindBuffer(_target, new BufferObjectHandle(0)));
    }

    [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
    public IMemoryOwner<T> MapRange(int offset, int length, BufferAccess flags = BufferAccess.None) {
        MapBufferAccessMask mask = 0;
        if ((flags & BufferAccess.Persistent) != 0)
            mask |= MapBufferAccessMask.MapPersistentBit;
        if ((flags & BufferAccess.Coherent) != 0)
            mask |= MapBufferAccessMask.MapCoherentBit;
        if ((flags & BufferAccess.Read) != 0)
            mask |= MapBufferAccessMask.MapReadBit;
        if ((flags & BufferAccess.Write) != 0)
            mask |= MapBufferAccessMask.MapWriteBit;
    
        return new BufferMappingRange<T>(this, _gl, offset, length, mask);
    }
}