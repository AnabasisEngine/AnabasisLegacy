using System.ComponentModel.DataAnnotations;

namespace Anabasis.Platform.Abstractions.Shaders;

[AttributeUsage(AttributeTargets.Field)]
public class ShaderUniformAttribute : Attribute
{
    public ShaderUniformAttribute(string name) {
        Name = name;
    }
    public string Name { get; }
    public int? Layout { get; set; }
}