namespace Anabasis.Graphics.Abstractions.Shaders;

public interface IShaderProgramTexts
{
    public IEnumerable<(ShaderType, Task<string>)> GetTexts();
}