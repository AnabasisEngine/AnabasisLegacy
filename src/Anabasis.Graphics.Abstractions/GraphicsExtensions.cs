using System.Diagnostics.CodeAnalysis;
using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Graphics.Abstractions.Textures;

namespace Anabasis.Graphics.Abstractions;

public static class GraphicsExtensions
{
    public static DrawPipeline CreateDrawPipeline(this IGraphicsDevice graphicsDevice, ShaderProgram shaderProgram) =>
        new(shaderProgram, graphicsDevice);

    public static async ValueTask<T> CompileShaderAsync<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        T>(this IShaderSupport support, CancellationToken cancellationToken = default)
        where T : ShaderProgram {
        T program = Activator.CreateInstance(typeof(T), support) as T ?? throw new InvalidOperationException();
        await program.CompileAsync(cancellationToken);
        return program;
    }

    public static ITextureBinding BindTexture(this IShaderParameter<ITextureBinding> parameter, ITexture texture,
        int unit = 0) => parameter.Value = texture.Bind(unit);
}