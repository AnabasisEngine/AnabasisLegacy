using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

public static class AnabasisGlOverloads
{
    public static void NamedBufferData<T>(this GL gl, uint buffer, ReadOnlySpan<T> data, BufferUsageARB usage)
        where T : unmanaged => gl.NamedBufferData(buffer, (nuint)(data.Length * Marshal.SizeOf<T>()), data,
        (VertexBufferObjectUsage)usage);

    public static void NamedBufferSubData<T>(this GL gl, uint buffer, nint offset, ReadOnlySpan<T> data)
        where T : unmanaged => gl.NamedBufferSubData(buffer, offset, (nuint)(data.Length * Marshal.SizeOf<T>()), data);
}