using System.Buffers;
using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Utility;
using Microsoft.Extensions.Options;

namespace Anabasis.Ascension.SpriteBatch;

public sealed class SpriteBatcher : IDisposable
{
    internal readonly DrawPipeline                               Pipeline;
    internal readonly SpriteShader                               Shader;
    private readonly  IMappableBufferObject<VertexSpecification> _vertexBuffer;
    private           int                                        _currentSyncOffset;
    private readonly  int                                        _syncLength;
    private readonly  IMappableBufferObject<ushort>              _indexBuffer;
    private readonly  IMemoryOwner<VertexSpecification>          _mappedVertexBuffer;
    private readonly  IMemoryOwner<ushort>                       _mappedIndexBuffer;

    private SpriteBatcher(DrawPipeline pipeline, SpriteShader shader,
        IMappableBufferObject<VertexSpecification> vertexBuffer,
        IMappableBufferObject<ushort> indexBuffer) {
        Pipeline = pipeline;
        Shader = shader;
        _vertexBuffer = vertexBuffer;
        _indexBuffer = indexBuffer;
        _syncLength = _vertexBuffer.Length / 3;
        _mappedVertexBuffer = _vertexBuffer.MapRange(0, _vertexBuffer.Length);
        _mappedIndexBuffer = _indexBuffer.MapRange(0, _indexBuffer.Length);
    }

    internal static async ValueTask<SpriteBatcher> CreateSpriteBatcher(IGraphicsDevice graphicsDevice,
        IShaderSupport shaderSupport, IOptions<SpriteBatchOptions> options) {
        SpriteShader shader = await shaderSupport.CompileShaderAsync<SpriteShader>();
        DrawPipeline pipeline = graphicsDevice.CreateDrawPipeline(shader);
        // 6/4 as many indices, times three for triple buffering
        IBufferObject<ushort> indexBuffer = pipeline.CreateIndexBuffer<ushort>(options.Value.VertexBufferSize * 9 / 2,
            flags: BufferAccess.Persistent | BufferAccess.Write | BufferAccess.Coherent);
        IBufferObject<VertexSpecification> vertexBuffer = pipeline.CreateVertexBuffer<VertexSpecification>(
            options.Value.VertexBufferSize * 3, flags: BufferAccess.Persistent | BufferAccess.Write | BufferAccess.Coherent);
        return new SpriteBatcher(pipeline, shader,
            Guard.IsType<IMappableBufferObject<VertexSpecification>>(vertexBuffer),
            Guard.IsType<IMappableBufferObject<ushort>>(indexBuffer));
    }

    public SpriteBatch Begin() {
        SpriteBatch batch = new(_mappedVertexBuffer.Memory.Span, _currentSyncOffset * _syncLength, _syncLength, this,
            _mappedIndexBuffer.Memory.Span);
        _currentSyncOffset = (_currentSyncOffset + 1) % 3;
        return batch;
    }

    public void Dispose() {
        Pipeline.Dispose();
        Shader.Dispose();
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _mappedVertexBuffer.Dispose();
        _mappedIndexBuffer.Dispose();
    }
}