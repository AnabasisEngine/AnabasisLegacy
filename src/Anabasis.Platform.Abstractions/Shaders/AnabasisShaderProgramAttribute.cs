using System.Diagnostics.CodeAnalysis;

namespace Anabasis.Platform.Abstractions.Shaders;

[AttributeUsage(AttributeTargets.Interface)]
public class AnabasisShaderProgramAttribute : Attribute
{
    public AnabasisShaderProgramAttribute(Type vertexType, Type textsType) {
        TextsType = textsType;
        VertexType = vertexType;
    }
    
    public Type VertexType { get; }
    public Type TextsType { get; }
}