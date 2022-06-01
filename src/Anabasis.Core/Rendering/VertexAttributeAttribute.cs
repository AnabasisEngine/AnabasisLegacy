namespace Anabasis.Core.Rendering;

[AttributeUsage(AttributeTargets.Field)]
public sealed class VertexAttributeAttribute : Attribute
{
    public string Name { get; }
    public bool Normalize { get; }

    public VertexAttributeAttribute(string name, bool normalize = false) {
        Name = name;
        Normalize = normalize;
    }
}