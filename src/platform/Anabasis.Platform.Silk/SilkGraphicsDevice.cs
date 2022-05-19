using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Platform.Abstractions;
using Anabasis.Platform.Silk.Buffers;
using Anabasis.Platform.Silk.Internal;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Anabasis.Platform.Silk;

internal class SilkGraphicsDevice : IDisposable, IGraphicsDevice
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IWindow        _contextSource;

    internal SilkGraphicsDevice(IAnabasisWindow window, ILoggerFactory loggerFactory) {
        _loggerFactory = loggerFactory;
        _contextSource = ((SilkWindow)window).Window;
    }

    private IGlApi? _gl;
    internal IGlApi Gl => Load();

    internal IGlApi Load() {
        return LazyInitializer.EnsureInitialized(ref _gl, () => new GlApi(GL.GetApi(_contextSource), _loggerFactory.CreateLogger<GlApi>()));
    }
    
    public void Dispose() {
        _gl?.Dispose();
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

    public IBufferObject<T> AllocateBuffer<T>(BufferType bufferType, int length, BufferAccess access)
        where T : unmanaged => new SilkBufferObject<T>(Gl, bufferType switch {
        BufferType.IndexBuffer => BufferTargetARB.ElementArrayBuffer,
        BufferType.VertexBuffer => BufferTargetARB.ArrayBuffer,
        _ => throw new ArgumentOutOfRangeException(nameof(bufferType), bufferType, null),
    }, length, access);
}