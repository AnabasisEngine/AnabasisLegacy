using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Platform.Silk.Internal;
using Anabasis.Platform.Silk.Textures;
using Anabasis.Utility;

namespace Anabasis.Platform.Silk.Shader.Parameters;

internal sealed class TextureParameter : SilkShaderParameter<ITextureBinding>
{
    public TextureParameter(IGlApi gl, string name, ProgramHandle program) : base(gl, name, program) { }
    protected override void SetValue(uint programHandle, int location, in ITextureBinding value) {
        Gl.ProgramUniform(programHandle, location, Guard.IsType<TextureBinding>(value).Unit);
    }
}