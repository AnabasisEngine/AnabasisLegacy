using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Platform.Abstractions;

namespace Anabasis.Graphics.Abstractions.Shaders;

public interface IShaderSupport
{
    ValueTask<IPlatformHandle> CompileAndLinkAsync(IGraphicsDevice provider, IShaderProgramTexts texts, CancellationToken 
    cancellationToken);

    IVertexBufferFormatter<TVertex> CreateVertexFormatter<TVertex>(IGraphicsDevice provider,
        IPlatformHandle programHandle)
        where TVertex : unmanaged;

    IShaderParameter<TParam> CreateParameter<TParam>(IGraphicsDevice graphicsDevice, string name,
        IPlatformHandle programHandle)
        where TParam : struct;
}