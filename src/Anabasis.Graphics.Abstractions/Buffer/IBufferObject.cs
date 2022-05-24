using Anabasis.Platform.Abstractions.Resources;

namespace Anabasis.Graphics.Abstractions.Buffer;

public interface IBufferObject<T> : IPlatformResource
    where T : unmanaged
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="offset"></param>
    /// <param name="flags"></param>
    /// <remarks>
    /// This call *may* change the actively bound buffer object(s) if necessary for the particular backend graphics api
    /// </remarks>
    void LoadData(ReadOnlySpan<T> data, int offset = 0, BufferAccess flags = BufferAccess.DefaultMap);

    /// <summary>
    /// Returns -1 if the buffer is not allocated
    /// </summary>
    int Length { get; }

    /// <summary>
    /// (Pre-)Allocates a buffer object of <paramref name="length"/> elements, configured using the given
    /// <see cref="BufferAccess"/> as storage hints
    /// </summary>
    void Allocate(int length, ReadOnlySpan<T> data = default, BufferAccess flags = BufferAccess.DefaultMap);

    /// <summary>
    /// Maps this buffer object to a span of at least <paramref name="length"/> elements and invokes
    /// <paramref name="load"/> to fill the buffer with data.
    /// </summary>
    /// <remarks>
    /// The default implementation of this method stack-allocates a block of <paramref name="length"/> elements
    /// and calls <see cref="LoadData(ReadOnlySpan{T}, int)"/>
    /// </remarks>
    void LoadData(int offset, int length, StatelessSpanAction<T> load,
        BufferAccess flags = BufferAccess.DefaultMap) {
        Span<T> buf = stackalloc T[length];
        load(buf);
        LoadData(buf, offset, flags);
    }
}