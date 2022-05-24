using System.Buffers;

namespace Anabasis.Graphics.Abstractions.Buffer;

public interface IMappableBufferObject<T> : IBufferObject<T>
    where T : unmanaged
{
    public IMemoryOwner<T> MapRange(int offset, int length, BufferAccess flags = BufferAccess.DefaultMap);

    void IBufferObject<T>.LoadData(int offset, int length, StatelessSpanAction<T> load, BufferAccess flags) {
        if (Length <= 0)
            Allocate(length + offset, flags: flags);
        using IMemoryOwner<T> owner = MapRange(offset, length, flags);
        load(owner.Memory.Span);
    }
}