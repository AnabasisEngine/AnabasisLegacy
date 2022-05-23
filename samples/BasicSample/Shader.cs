using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Graphics.Abstractions.Textures;

namespace BasicSample;

public class Shader : ShaderProgram
{
    public Shader(IShaderSupport support) : base(support) {
        
    }

    private IShaderParameter<ITextureBinding>? _textureParameter;
    public IShaderParameter<ITextureBinding> TextureArray => CreateParameter(ref _textureParameter, "texarray");

    public override IEnumerable<(ShaderType,Task<string>)> GetTexts() {
        yield return (ShaderType.Fragment, File.ReadAllTextAsync("shader.frag"));
        yield return (ShaderType.Vertex, File.ReadAllTextAsync("shader.vert"));
    }
}