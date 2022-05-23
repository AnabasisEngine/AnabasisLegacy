using System.Numerics;
using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Platform.Abstractions;
using Silk.NET.Maths;

namespace Anabasis.Graphics.Abstractions;

public interface IGraphicsDevice
{
    public IVertexArray CreateVertexArray();

    public IBufferObject<T> CreateBuffer<T>(BufferType bufferType)
        where T : unmanaged;

    void Clear(Color color, ClearFlags flags = ClearFlags.None);
    
    Vector2D<uint> Viewport { get; set; }
}

[Flags]
public enum ClearFlags
{
    None     = 0,
    Color    = 1,
    Stencil  = 2,
    Depth    = 4,
    Coverage = 8
}