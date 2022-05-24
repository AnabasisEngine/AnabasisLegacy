using System.Numerics;
using Anabasis.Abstractions;
using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Threading;
using Microsoft.Extensions.Logging;

namespace InstancingSample;

[VertexType]
public struct VertexData
{
    [VertexAttribute("aPos", Layout = 0)]
    public Vector2 Pos;

    [VertexAttribute("aColor", Layout = 1, Normalize = true)]
    public Color Color;
}

[VertexType(Divisor = 1)]
public struct InstanceData
{
    [VertexAttribute("aOffset", Layout = 2)]
    public Vector2 Offset;
}

public class Game : IAnabasisGame, IDisposable
{
    private readonly ILogger<Game>        _logger;
    private readonly IGraphicsDevice      _graphics;
    private readonly IShaderSupport       _shaderSupport;
    private readonly TaskCompletionSource _loadedTcs;

    private DrawPipeline _pipeline = null!;

    public Game(ILogger<Game> logger, IGraphicsDevice graphics, IShaderSupport shaderSupport) {
        _logger = logger;
        _graphics = graphics;
        _shaderSupport = shaderSupport;
        _loadedTcs = new TaskCompletionSource();
    }

    public async AnabasisCoroutine Load() {
        _logger.LogDebug("Game.Load");

        _pipeline = _graphics.CreateDrawPipeline(await _shaderSupport.CompileShaderAsync<Shader>());

        _pipeline.CreateVertexBuffer<VertexData>(6, CreateInstanceVertices);
        _pipeline.CreateVertexBuffer<InstanceData>(100, CreateInstanceTransforms);

        _loadedTcs.SetResult();
    }

    private static void CreateInstanceTransforms(Span<InstanceData> data) {
        int index = 0;
        float offset = 0.1f;
        for (int y = -10; y < 10; y += 2) {
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
    }

    private static void CreateInstanceVertices(Span<VertexData> data) {
        data[0].Pos = new Vector2(-0.5f, 0.5f);
        data[0].Color = Color.Red;

        data[1].Pos = new Vector2(0.5f, -0.5f);
        data[1].Color = Color.Green;

        data[2].Pos = new Vector2(-0.5f, -0.5f);
        data[2].Color = Color.Blue;

        data[3].Pos = new Vector2(-0.5f, 0.5f);
        data[3].Color = Color.Red;

        data[4].Pos = new Vector2(0.5f, -0.5f);
        data[4].Color = Color.Green;

        data[5].Pos = new Vector2(0.5f, 0.5f);
        data[5].Color = Color.Blue;
    }

    public void Update() { }

    public void Render() {
        if (!_loadedTcs.Task.IsCompleted)
            return;
        _graphics.Clear(new Color(0.1f, 0.1f, 0.1f, 1f), ClearFlags.Color | ClearFlags.Depth);
        _pipeline.DrawArraysInstanced(DrawMode.Triangles, 0, 6, 100);
    }

    public void Dispose() {
        _pipeline.Dispose();
    }
}