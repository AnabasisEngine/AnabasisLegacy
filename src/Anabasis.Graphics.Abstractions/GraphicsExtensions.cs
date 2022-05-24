using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Graphics.Abstractions.Internal;
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

    public static void LoadData<T, TArg>(this IBufferObject<T> buffer, int offset, int length,
        SpanAction<T, TArg> action, TArg state)
        where T : unmanaged {
        using (PooledSpanActions.GetPooledAction(action, state, out StatelessSpanAction<T> boundAction))
            buffer.LoadData(offset, length, boundAction);
    }

    public static IBufferObject<T> CreateVertexBuffer<T, TArg>(this DrawPipeline pipeline, int count,
        SpanAction<T, TArg> action, TArg state, BufferAccess flags = BufferAccess.DefaultMap,
        IVertexBufferFormatter<T>? formatter = null)
        where T : unmanaged {
        using (PooledSpanActions.GetPooledAction(action, state, out StatelessSpanAction<T> boundAction))
            return pipeline.CreateVertexBuffer(count, boundAction, flags);
    }

    public static IBufferObject<T> CreateIndexBuffer<T, TArg>(this DrawPipeline pipeline, int count,
        SpanAction<T, TArg> action, TArg state, BufferAccess flags = BufferAccess.DefaultMap,
        IVertexBufferFormatter<T>? formatter = null)
        where T : unmanaged {
        using (PooledSpanActions.GetPooledAction(action, state, out StatelessSpanAction<T> boundAction))
            return pipeline.CreateIndexBuffer(count, boundAction, flags);
    }
}