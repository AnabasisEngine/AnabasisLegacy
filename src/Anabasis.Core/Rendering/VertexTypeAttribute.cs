namespace Anabasis.Core.Rendering;

[AttributeUsage(AttributeTargets.Struct)]
public sealed class VertexTypeAttribute : Attribute
{
    public VertexTypeAttribute(string vertexShaderFile, int divisor = 0) {
        VertexShaderFile = vertexShaderFile;
        Divisor = divisor;
    }

    public string VertexShaderFile { get; }
    public int Divisor { get; }
}