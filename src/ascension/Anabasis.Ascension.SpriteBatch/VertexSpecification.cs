using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Shaders;
using Silk.NET.Maths;

namespace Anabasis.Ascension.SpriteBatch;

[VertexType]
internal struct VertexSpecification
{
    [VertexAttribute("pos")]
    public Vector3D<float> Position;

    [VertexAttribute("color", Normalize = true)]
    public Color Color;

    [VertexAttribute("uv", Normalize = true)]
    public Vector2D<ushort> TexCoord;
}