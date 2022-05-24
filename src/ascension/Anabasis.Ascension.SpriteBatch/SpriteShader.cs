using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Graphics.Abstractions.Textures;

namespace Anabasis.Ascension.SpriteBatch;

internal class SpriteShader : ShaderProgram
{
    public SpriteShader(IShaderSupport support) : base(support) { }

    private IShaderParameter<ITextureBinding>? _texture;
    public IShaderParameter<ITextureBinding> Texture => CreateParameter(ref _texture, "tex");
    public override IEnumerable<(ShaderType, Task<string>)> GetTexts() => throw new NotImplementedException();
}