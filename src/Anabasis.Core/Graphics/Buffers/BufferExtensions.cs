using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Buffers;

[SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
[SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters")]
public static class BufferExtensions
{
    public static void LoadData<T>(this GraphicsBuffer buffer, StatelessSpanAction<T> action, int offset, int length,
        MapBufferAccessMask mask = MapBufferAccessMask.MapCoherentBit | MapBufferAccessMask.MapPersistentBit | MapBufferAccessMask.MapWriteBit)
        where T : unmanaged {
        using IMemoryOwner<T> slice = buffer.MapSlice<T>(offset, length, mask);
        action(slice.Memory.Span);
    }

    public static void LoadData<T, TArg>(this GraphicsBuffer buffer, SpanAction<T, TArg> action, TArg arg, int offset,
        int length, MapBufferAccessMask mask = MapBufferAccessMask.MapCoherentBit | MapBufferAccessMask.MapPersistentBit | MapBufferAccessMask.MapWriteBit)
        where T : unmanaged {
        using (PooledDelegates.GetPooledSpanAction(action, arg, out StatelessSpanAction<T> act))
            buffer.LoadData(act, offset, length, mask);
    }

    public static void CopyTo<T>(this ReadOnlySpan<T> span, GraphicsBuffer.TypedBufferSlice<T> slice)
        where T : unmanaged {
        slice.Write(span);
    }

    public static void CopyTo<T>(this ReadOnlySpan<T> span, GraphicsBuffer buffer)
        where T : unmanaged {
        buffer.Typed<T>().Write(span);
    }
}