using System.Numerics;
using Anabasis.Graphics.Abstractions;

namespace Anabasis.Ascension;

public class Camera2D
{
    private readonly IGraphicsDevice _graphics;

    public Camera2D(IGraphicsDevice graphics) {
        _graphics = graphics;
    }

    public Vector2 Position {
        get => _position;
        set {
            _needsUpdate = true;
            _position = value;
        }
    }

    public float Zoom {
        get => _zoom;
        set {
            _needsUpdate = true;
            _zoom = value;
        }
    }

    public Matrix4x4 Transform {
        get {
            if (_needsUpdate)
                _transform = Matrix4x4.CreateTranslation(-Position.X, -Position.Y, 0)
                             * Matrix4x4.CreateScale(Zoom)
                             * Matrix4x4.CreateTranslation(_graphics.Viewport.X / 2f, _graphics.Viewport.Y / 2f, 0);
            return _transform;
        }
    }

    private bool      _needsUpdate = true;
    private float     _zoom;
    private Vector2   _position;
    private Matrix4x4 _transform;
}