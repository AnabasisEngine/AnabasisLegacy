namespace Anabasis.Platform.Abstractions.Shaders;

public interface IShaderProgramTexts
{
    public Dictionary<ShaderType, IAsyncEnumerable<string>> GetTexts();
}