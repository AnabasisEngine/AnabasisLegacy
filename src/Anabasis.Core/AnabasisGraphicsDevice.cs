using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Anabasis.Core;

public sealed class AnabasisGraphicsDevice : IDisposable
{
    private Vector2D<int> _viewport;
    public GL GL { get; internal set; } = null!;

    public Vector2D<int> Viewport {
        get => _viewport;
        set {
            GL.Viewport(value);
            _viewport = value;
        }
    }

    public void Dispose() {
        GL.Dispose();
    }
}