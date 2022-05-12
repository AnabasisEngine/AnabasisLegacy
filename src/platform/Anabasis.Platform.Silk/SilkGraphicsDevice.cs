using Anabasis.Platform.Abstractions;
using Anabasis.Platform.Abstractions.Buffer;
using Anabasis.Platform.Silk.Buffers;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk;

internal class SilkGraphicsDevice : IDisposable, IGraphicsDevice
{
    internal SilkGraphicsDevice(IAnabasisWindow window) {
        Gl = GL.GetApi(((SilkWindow)window).Window);
    }

    internal GL Gl { get; }

    public void Dispose() {
        Gl.Dispose();
    }

    public IVertexArray<TVertex, TIndex> CreateVertexArray<TVertex, TIndex>()
        where TVertex : unmanaged
        where TIndex : unmanaged => new SilkVertexArray<TVertex, TIndex>(Gl);

    public IBufferObject<T> CreateBuffer<T>(BufferType bufferType)
        where T : unmanaged => new SilkBufferObject<T>(Gl, bufferType switch {
        BufferType.IndexBuffer => BufferTargetARB.ElementArrayBuffer,
        BufferType.VertexBuffer => BufferTargetARB.ArrayBuffer,
        _ => throw new ArgumentOutOfRangeException(nameof(bufferType), bufferType, null),
    });
}