using Anabasis.Graphics.Abstractions.Shaders;

namespace InstancingSample;

public class Shader : ShaderProgram
{
    public Shader(IShaderSupport support) : base(support) {
        
    }

    public override IEnumerable<(ShaderType,Task<string>)> GetTexts() {
        yield return (ShaderType.Fragment, File.ReadAllTextAsync("shader.frag"));
        yield return (ShaderType.Vertex, File.ReadAllTextAsync("shader.vert"));
    }
}