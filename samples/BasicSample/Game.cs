using System.Numerics;
using Anabasis.Abstractions;
using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Images.Abstractions;
using Anabasis.Threading;
using Microsoft.Extensions.Logging;
using Silk.NET.Maths;

namespace BasicSample;

[VertexType]
public struct VertexData
{
    [VertexAttribute("aPos", Layout = 0)]
    public Vector2 Pos;

    [VertexAttribute("aColor", Layout = 1)]
    public Vector3 Color;

    [VertexAttribute("aTexCoord")]
    public Vector2 TexCoord;
}

[VertexType(Divisor = 1)]
public struct InstanceData
{
    [VertexAttribute("aOffset")]
    public Vector2 Offset;

    [VertexAttribute("aTexLayer")]
    public int LayerCoord;
}

public class Game : IAnabasisGame, IDisposable
{
    private readonly ILogger<Game>        _logger;
    private readonly IAnabasisTime        _time;
    private readonly IGraphicsDevice      _graphics;
    private readonly ITextureSupport      _textureSupport;
    private readonly IShaderSupport       _shaderSupport;
    private readonly AnabasisTaskManager  _taskManager;
    private readonly IImageDataLoader     _imageLoader;
    private readonly TaskCompletionSource _loadedTcs;
    private          Shader               _shader       = null!;
    private          DrawPipeline         _drawPipeline = null!;

    public Game(ILogger<Game> logger, IAnabasisTime time, IGraphicsDevice graphics, ITextureSupport textureSupport,
        IShaderSupport shaderSupport, AnabasisTaskManager taskManager, IImageDataLoader imageLoader) {
        _logger = logger;
        _time = time;
        _graphics = graphics;
        _textureSupport = textureSupport;
        _shaderSupport = shaderSupport;
        _taskManager = taskManager;
        _imageLoader = imageLoader;
        _loadedTcs = new TaskCompletionSource();
    }

    public async AnabasisCoroutine Load() {
        _graphics.Viewport = new Vector2D<uint>(1280, 720);
        _logger.LogDebug("Game.Load");
        await _taskManager.Yield(AnabasisPlatformStepMask.PostInitialization);
        _logger.LogDebug("Game.Load in PostInitialization");

        _shader = await _shaderSupport.CompileShaderAsync<Shader>();
        _drawPipeline = _graphics.CreateDrawPipeline(_shader);

        LoadInstanceTransforms(_drawPipeline);
        LoadInstanceVertices(_drawPipeline);

        _loadedTcs.SetResult();
    }

    private void LoadInstanceTransforms(DrawPipeline instances) {
        Span<InstanceData> data = stackalloc InstanceData[100];
        const float offset = 0.1f;
        for (int index = 0, y = -10; y < 10; y += 2) {
            for (int x = -10; x < 10; x += 2) {
                // glm::vec2 translation;
                // translation.x = (float)x / 10.0f + offset;
                // translation.y = (float)y / 10.0f + offset;
                // translations[index++] = translation;
                ref InstanceData instance = ref data[index++];
                instance.Offset.X = x / 10.0f + offset;
                instance.Offset.Y = y / 10.0f + offset;
            }
        }

        instances.CreateVertexBuffer<InstanceData>(100, data);
    }

    private void LoadInstanceVertices(DrawPipeline vertices) {
        Span<VertexData> data = stackalloc VertexData[6];
        data[0].Pos = new Vector2(-0.5f, 0.5f);
        data[0].Color = new Vector3(1f, 0f, 0f);

        data[1].Pos = new Vector2(0.5f, -0.5f);
        data[1].Color = new Vector3(0f, 1f, 0f);

        data[2].Pos = new Vector2(-0.5f, -0.5f);
        data[2].Color = new Vector3(0f, 0f, 1f);

        data[3].Pos = new Vector2(-0.5f, 0.5f);
        data[3].Color = new Vector3(1f, 0f, 0f);

        data[4].Pos = new Vector2(0.5f, -0.5f);
        data[4].Color = new Vector3(0f, 1f, 0f);

        data[5].Pos = new Vector2(0.5f, 0.5f);
        data[5].Color = new Vector3(0f, 0f, 1f);
        vertices.CreateVertexBuffer<VertexData>(6, data);
    }

    public void Update() { }

    public void Render() {
        if (!_loadedTcs.Task.IsCompleted)
            return;
        _graphics.Clear(new Color(0.1f, 0.1f, 0.1f, 1f), ClearFlags.Color | ClearFlags.Depth);
        _drawPipeline.DrawArraysInstanced(DrawMode.Triangles, 0, 6, 100);
    }

    public void Dispose() {
        GC.SuppressFinalize(this);
        _drawPipeline.Dispose();
        _shader.Dispose();
    }
}