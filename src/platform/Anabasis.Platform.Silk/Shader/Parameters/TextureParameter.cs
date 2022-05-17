using Anabasis.Platform.Silk.Internal;
using Anabasis.Platform.Silk.Textures;

namespace Anabasis.Platform.Silk.Shader.Parameters;

internal class TextureParameter : SilkShaderParameter<TextureBinding>
{
    public TextureParameter(IGlApi gl, string name, ProgramHandle program) : base(gl, name, program) { }
    protected override void SetValue(uint programHandle, int location, in TextureBinding value) {
        Gl.ProgramUniform(programHandle, location, value.Unit);
    }
}