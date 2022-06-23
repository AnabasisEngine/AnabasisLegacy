using System.Numerics;
using System.Runtime.InteropServices;
using Anabasis.Core.Graphics.Rendering;
using Silk.NET.OpenGL;

namespace AscensionSample;

[VertexType]
[StructLayout(LayoutKind.Sequential)]
public partial struct Vertex
{
    [VertexAttribute("vPos", AttributeType.FloatVec3, 0)]
    public Vector3 Position;
    [VertexAttribute("vUv", AttributeType.FloatVec2, 1)]
    public Vector2 TexCoord;
}