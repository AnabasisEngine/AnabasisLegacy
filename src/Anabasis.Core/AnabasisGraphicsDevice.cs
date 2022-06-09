using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Anabasis.Core;

public sealed class AnabasisGraphicsDevice : IDisposable
{
    private Rectangle<int>? _viewport;
    public GL Gl { get; internal set; } = null!;

    public Rectangle<int> ViewportRect {
        get => _viewport ??= GetViewport();
        set {
            Gl.Viewport(value);
            _viewport = value;
        }
    }

    public Vector2D<int> ViewportSize {
        get => ViewportRect.Size;
        set => ViewportRect = new Rectangle<int>(ViewportRect.Origin, value);
    }

    private Rectangle<int> GetViewport() {
        Rectangle<int> rect = default;
        Gl.GetInteger(GetPName.Viewport,rect.AsSpan());
        return rect;
    }

    public void Dispose() {
        Gl.Dispose();
    }
}