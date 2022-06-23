using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics;

public sealed class AnabasisGraphicsDevice : IDisposable
{
    private Rectangle<int>? _viewport;
    public GL Gl { get; internal set; } = null!;

    public Rectangle<int> ViewportRect {
        get => _viewport ??= GetViewport();
        set {
            if(value == _viewport) return;
            Gl.Viewport(value);
            _viewport = value;
            ViewportChanged?.Invoke();
        }
    }

    public Vector2D<int> ViewportSize {
        get => ViewportRect.Size;
        set {
            if(ViewportRect.Size == value) return;
            ViewportRect = new Rectangle<int>(ViewportRect.Origin, value);
            ViewportChanged?.Invoke();
        }
    }

    private Rectangle<int> GetViewport() {
        Rectangle<int> rect = default;
        Gl.GetInteger(GetPName.Viewport,rect.AsSpan());
        return rect;
    }

    public event Action? ViewportChanged; 

    public void Dispose() {
        Gl.Dispose();
    }
}