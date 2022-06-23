namespace Anabasis.Core.Graphics.Rendering;

[AttributeUsage(AttributeTargets.Struct)]
public sealed class VertexTypeAttribute : Attribute
{
    public VertexTypeAttribute(int divisor = 0) {
        Divisor = divisor;
    }

    public int Divisor { get; }
}