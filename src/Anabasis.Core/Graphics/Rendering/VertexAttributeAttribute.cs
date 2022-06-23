using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Rendering;

[AttributeUsage(AttributeTargets.Field)]
public sealed class VertexAttributeAttribute : Attribute
{
    public string Name { get; }
    
    public AttributeType AttributeType { get; }
    
    public int Layout { get; }
    public bool Normalize { get; }

    public VertexAttributeAttribute(string name, AttributeType attributeType, int layout, bool normalize = false) {
        Name = name;
        AttributeType = attributeType;
        Layout = layout;
        Normalize = normalize;
    }
}