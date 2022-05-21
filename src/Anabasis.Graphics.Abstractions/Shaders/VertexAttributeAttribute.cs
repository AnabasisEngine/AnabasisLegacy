namespace Anabasis.Graphics.Abstractions.Shaders;

[AttributeUsage(AttributeTargets.Field)]
public class VertexAttributeAttribute : Attribute
{
    public VertexAttributeAttribute(string name) {
        Name = name;
    }
    public string Name { get; }
    public int Layout { get; set; } = -1;
}

[AttributeUsage(AttributeTargets.Struct)]
public class VertexTypeAttribute : Attribute
{
    public uint Divisor { get; set; } = 0;
}