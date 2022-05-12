namespace Anabasis.Platform.Abstractions.Shaders;

public interface IShaderProgramSource<TShader, TVertex>
    where TShader : IShaderProgram<TVertex>
    where TVertex : unmanaged
{
    public ValueTask<TShader> CompileAndLinkAsync(CancellationToken cancellationToken);
}