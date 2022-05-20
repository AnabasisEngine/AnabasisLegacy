using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Platform.Silk.Internal;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Buffers;

public class SilkBufferObject<T> : SilkGlObject<BufferObjectHandle>, IBufferObject<T>
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
                "Cannot change the size of a buffer after first allocation, use Allocate()");
        }
    }

    public void LoadData<TArg>(int offset, int length, SpanAction<T, TArg> load, TArg state) {
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), offset, null);
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), length, null);
        if (Length < 0) {
            Allocate(length);
        }

        if (Length < offset + Length)
            throw new ArgumentOutOfRangeException(nameof(length), length, null);
        Range range = offset..(offset + length);
        LoadData(range, load, state);
    }
    
    public void LoadData<TArg>(Range range, SpanAction<T, TArg> load, TArg state) {
        using BufferMappingRange<T> mapping = MapRange(range, BufferAccess.Write | BufferAccess.Coherent | BufferAccess.Persistent);
        load(mapping.Span, state);
    }

    public override void Dispose() {
        GC.SuppressFinalize(this);
        _gl.DeleteBuffer(Handle);
    }

    public override void Use() {
        _gl.BindBuffer(_target, Handle);
    }

    [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
    public BufferMappingRange<T> MapRange(Range range, BufferAccess flags = BufferAccess.None) {
        MapBufferAccessMask mask = 0;
        if ((flags & BufferAccess.Persistent) != 0)
            mask |= MapBufferAccessMask.MapPersistentBit;
        if ((flags & BufferAccess.Coherent) != 0)
            mask |= MapBufferAccessMask.MapCoherentBit;
        if ((flags & BufferAccess.Read) != 0)
            mask |= MapBufferAccessMask.MapReadBit;
        if ((flags & BufferAccess.Write) != 0)
            mask |= MapBufferAccessMask.MapWriteBit;

        return new BufferMappingRange<T>(this, _gl, range, mask);
    }

    public BufferMapping<T> Map(BufferAccess flags = BufferAccess.None) => new(this, _gl, flags switch {
            BufferAccess.Read => BufferAccessARB.ReadOnly,
            BufferAccess.Write => BufferAccessARB.WriteOnly,
            BufferAccess.ReadWrite => BufferAccessARB.ReadWrite,
            _ => throw new ArgumentOutOfRangeException(nameof(flags), flags,
                "Cannot map full buffer with given flags, use MapRange(Range.All)"),
        });
}