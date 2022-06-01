using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Anabasis.Core.Handles;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Buffers;

public sealed class GraphicsBuffer : AnabasisNativeObject<BufferHandle>
{
    public GraphicsBuffer(GL gl) : base(gl, new BufferHandle(gl.CreateBuffer())) { }

    public IDisposable Use(BufferTargetARB target) {
        Gl.BindBuffer(target, Handle.Value);
        return new GenericDisposer(() => Gl.BindBuffer(target, 0));
    }

    [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
    public void AllocateBuffer(int bytes) {
        ThrowIfAlreadyAllocated();
        Gl.NamedBufferStorage(Handle.Value, (nuint)bytes, ReadOnlySpan<byte>.Empty,
            BufferStorageMask.DynamicStorageBit | BufferStorageMask.MapCoherentBit |
            BufferStorageMask.MapPersistentBit | BufferStorageMask.MapWriteBit);
        Gl.ThrowIfError(nameof(Gl.NamedBufferStorage));
        Length = bytes;
    }

    [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
    public void AllocateBuffer<T>(int count = -1, ReadOnlySpan<T> initialData = default)
        where T : unmanaged {
        ThrowIfAlreadyAllocated();
        count = count switch {
            > 0 when count < initialData.Length =>
                throw new ArgumentOutOfRangeException(nameof(count), count,
                    "Cannot allocate less space than in the given initial data"),
            < 0 => initialData.Length,
            _ => count,
        };

        count *= Marshal.SizeOf<T>();
        Gl.NamedBufferStorage(Handle.Value, (nuint)count, MemoryMarshal.AsBytes(initialData),
            BufferStorageMask.DynamicStorageBit | BufferStorageMask.MapCoherentBit |
            BufferStorageMask.MapPersistentBit | BufferStorageMask.MapWriteBit);
        Gl.ThrowIfError(nameof(Gl.NamedBufferStorage));
        Length = count;
    }

    private void ThrowIfAlreadyAllocated() {
        if (Length > 0) {
            throw new InvalidOperationException("Cannot change buffer size after first allocation");
        }
    }

    public int Length { get; set; }

    public TypedBufferSlice<T> Slice<T>(int offset, int length)
        where T : unmanaged {
        if (length > Length)
            throw new ArgumentOutOfRangeException(nameof(length), length, "");
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "");
        return new TypedBufferSlice<T>(this, offset, length);
    }

    [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
    public IMemoryOwner<T> MapSlice<T>(int offset, int length,
        MapBufferAccessMask flags = MapBufferAccessMask.MapCoherentBit | MapBufferAccessMask.MapPersistentBit |
                                    MapBufferAccessMask.MapWriteBit | MapBufferAccessMask.MapReadBit)
        where T : unmanaged {
        if (length > Length)
            throw new ArgumentOutOfRangeException(nameof(length), length, "");
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "");
        return new BufferSliceMapping<T>(Handle, Gl, offset, length, flags);
    }

    public TypedBufferSlice<T> Typed<T>()
        where T : unmanaged => this;

    public readonly struct TypedBufferSlice<T>
        where T : unmanaged
    {
        private readonly GraphicsBuffer _buffer;
        private readonly int            _offset;
        private readonly int            _length;

        internal TypedBufferSlice(GraphicsBuffer buffer, int offset, int length) {
            _buffer = buffer;
            _offset = offset;
            _length = length;
        }

        public TypedBufferSlice<T> Slice(int offset, int length) {
            if (length > _length)
                throw new ArgumentOutOfRangeException(nameof(length), length, "");
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "");
            return new TypedBufferSlice<T>(_buffer, _offset + offset, length);
        }

        public void Write(ReadOnlySpan<T> span) {
            _buffer.Gl.NamedBufferSubData(_buffer.Handle.Value, _offset * Marshal.SizeOf<T>(),
                (nuint)(span.Length * Marshal.SizeOf<T>()), span);
        }

        [SuppressMessage("ReSharper", "ReplaceSliceWithRangeIndexer")]
        public static implicit operator  TypedBufferSlice<T>(GraphicsBuffer buffer) => buffer.Slice<T>(0, buffer.Length);
    }
}