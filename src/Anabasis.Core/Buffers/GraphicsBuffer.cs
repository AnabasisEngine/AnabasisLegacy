using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Anabasis.Core.Handles;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Buffers;

public sealed class GraphicsBuffer : AnabasisNativeObject<BufferHandle>
{
    public GraphicsBuffer(GL gl) : base(gl, new BufferHandle(gl.CreateBuffer())) { }

    /// <summary>
    /// This overload, while not strictly necessary, allows the graphics drivers to make assumptions about the nature of
    /// the buffer which may under some circumstances lead to improved performance
    /// </summary>
    public GraphicsBuffer(GL gl, BufferTargetARB target) : base(gl, new BufferHandle(gl.GenBuffer())) {
        Gl.BindBuffer(target, Handle.Value);
    }

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
    public void AllocateBuffer<T>(int count = -1, ReadOnlySpan<T> initialData = default,
        BufferStorageMask mask = BufferStorageMask.MapCoherentBit | BufferStorageMask.MapPersistentBit)
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
        if (initialData.IsEmpty) mask |= BufferStorageMask.DynamicStorageBit;
        Gl.NamedBufferStorage(Handle.Value, (nuint)count, MemoryMarshal.AsBytes(initialData),
            mask | BufferStorageMask.MapWriteBit);
        Gl.ThrowIfError(nameof(Gl.NamedBufferStorage));
        Length = count;
    }

    private void ThrowIfAlreadyAllocated() {
        if (Length > 0) {
            throw new InvalidOperationException("Cannot change buffer size after first allocation");
        }
    }

    public void BindRange(BufferTargetARB target, uint index, int offset, uint length) {
        VerifyTargetAndIndex(target, index);
        Gl.BindBufferRange(target, index, Handle.Value, offset, length);
        Gl.ThrowIfError(nameof(Gl.BindBufferRange));
    }

    public void BindIndex(BufferTargetARB target, uint index) {
        VerifyTargetAndIndex(target, index);
        Gl.BindBufferBase(target, index, Handle.Value);
        Gl.ThrowIfError(nameof(Gl.BindBufferBase));
    }

    private static uint? _maxTransformFeedback;

    private static uint? _maxUniformBindings;

    private static uint? _maxAtomicCounterBufferBindings;

    private static uint? _maxShaderStorageBufferBindings;

    private void VerifyTargetAndIndex(BufferTargetARB target, uint index) {
        uint max = target switch {
            BufferTargetARB.ShaderStorageBuffer => _maxShaderStorageBufferBindings ??=
                (uint)Gl.GetInteger(GLEnum.MaxShaderStorageBufferBindings),
            BufferTargetARB.TransformFeedbackBuffer => _maxTransformFeedback ??=
                (uint)Gl.GetInteger(GLEnum.MaxShaderStorageBufferBindings),
            BufferTargetARB.UniformBuffer => _maxUniformBindings ??=
                (uint)Gl.GetInteger(GLEnum.MaxShaderStorageBufferBindings),
            BufferTargetARB.AtomicCounterBuffer => _maxAtomicCounterBufferBindings ??=
                (uint)Gl.GetInteger(GLEnum.MaxShaderStorageBufferBindings),
            BufferTargetARB.ParameterBuffer or BufferTargetARB.ArrayBuffer or BufferTargetARB.ElementArrayBuffer
                or BufferTargetARB.PixelPackBuffer or BufferTargetARB.PixelUnpackBuffer or BufferTargetARB.TextureBuffer
                or BufferTargetARB.CopyReadBuffer or BufferTargetARB.CopyWriteBuffer
                or BufferTargetARB.DrawIndirectBuffer or BufferTargetARB.DispatchIndirectBuffer
                or BufferTargetARB.QueryBuffer => throw new ArgumentOutOfRangeException(nameof(target), target,
                    "Invalid target for binding buffer to index"),
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };

        if (index > max)
            throw new ArgumentOutOfRangeException(nameof(index), index,
                $"Index too large for binding buffer of type {target}");
    }

    public int Length { get; set; }

    public TypedBufferSlice<T> Slice<T>(int offset, int length)
        where T : unmanaged {
        if (length < 0)
            length = Length - offset;
        if (offset + length > Length)
            throw new ArgumentOutOfRangeException(nameof(length), length, "");
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "");
        return new TypedBufferSlice<T>(this, offset, length);
    }

    [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
    public IMemoryOwner<T> MapSlice<T>(int offset, int length,
        MapBufferAccessMask flags = MapBufferAccessMask.MapCoherentBit | MapBufferAccessMask.MapPersistentBit |
                                    MapBufferAccessMask.MapWriteBit)
        where T : unmanaged {
        if (length > Length)
            throw new ArgumentOutOfRangeException(nameof(length), length, "");
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "");
        return new BufferSliceMapping<T>(Handle, Gl, offset, length, flags);
    }

    public TypedBufferSlice<T> Typed<T>()
        where T : unmanaged => new(this, 0, Length);

    public readonly struct TypedBufferSlice<T>
        where T : unmanaged
    {
        public readonly GraphicsBuffer Buffer;
        public readonly int            Offset;
        public readonly int            Length;

        internal TypedBufferSlice(GraphicsBuffer buffer, int offset, int length) {
            Buffer = buffer;
            Offset = offset;
            Length = length;
        }

        public TypedBufferSlice<T> Slice(int offset = 0, int length = -1) {
            if (length < 0)
                length = Length - offset;
            if (offset + length > Length)
                throw new ArgumentOutOfRangeException(nameof(length), length, "");
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "");
            return new TypedBufferSlice<T>(Buffer, Offset + offset, length);
        }

        public void Write(ReadOnlySpan<T> span) {
            Buffer.Gl.NamedBufferSubData(Buffer.Handle.Value, Offset * Marshal.SizeOf<T>(),
                (nuint)(span.Length * Marshal.SizeOf<T>()), span);
        }

        public void BindIndex(BufferTargetARB target, uint index) {
            Buffer.BindRange(target, index, Offset, (uint)Length);
        }

        public static implicit operator TypedBufferSlice<T>(GraphicsBuffer buffer) => buffer.Typed<T>();
    }
}