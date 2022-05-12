using Anabasis.Platform.Abstractions.Buffer;

namespace Anabasis.Platform.Abstractions.Shaders;

public interface IShaderSupport
{
    ValueTask<IGraphicsHandle> CompileAndLinkAsync(IGraphicsDevice provider, IShaderProgramTexts texts, CancellationToken 
    cancellationToken);

    IVertexBufferFormatter<TVertex> CreateVertexFormatter<TVertex>(IGraphicsDevice provider,
        IGraphicsHandle programHandle)
        where TVertex : unmanaged;
}