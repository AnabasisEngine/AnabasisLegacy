using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Graphics.Abstractions.Textures;

namespace TexturedSample;

public class Shader : ShaderProgram
{
    public Shader(IShaderSupport support) : base(support) { }

    private IShaderParameter<ITextureBinding>? _textureParam;
    public IShaderParameter<ITextureBinding> TextureUniform => CreateParameter(ref _textureParam, "uTexture0");

    public override IEnumerable<(ShaderType, Task<string>)> GetTexts() {
        yield return (ShaderType.Vertex, File.ReadAllTextAsync("shader.vert"));
        yield return (ShaderType.Fragment, File.ReadAllTextAsync("shader.frag"));
    }
}