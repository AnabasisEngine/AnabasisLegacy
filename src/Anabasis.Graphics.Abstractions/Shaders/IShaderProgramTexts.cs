namespace Anabasis.Graphics.Abstractions.Shaders;

public interface IShaderProgramTexts
{
    public Dictionary<ShaderType, IAsyncEnumerable<string>> GetTexts();
}