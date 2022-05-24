using System.Buffers;

namespace Anabasis.Graphics.Abstractions.Buffer;

public interface IMappableBufferObject<T> : IBufferObject<T>
    where T : unmanaged
{
    public IMemoryOwner<T> MapRange(int offset, int length, BufferAccess flags);

    void IBufferObject<T>.LoadData<TArg>(int offset, int length, SpanAction<T, TArg> load, TArg state) {
        using IMemoryOwner<T> owner = MapRange(offset, length,
            BufferAccess.Coherent | BufferAccess.Dynamic | BufferAccess.Write);
        load(owner.Memory.Span, state);
    }
}