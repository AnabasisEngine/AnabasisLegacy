using System.Numerics;
using Anabasis.Abstractions;
using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Threading;
using Microsoft.Extensions.Logging;

namespace BasicSample;

[VertexType]
public struct VertexData
{
    [VertexAttribute("aPos", Layout = 0)]
    public Vector2 Pos;

    [VertexAttribute("aColor", Layout = 1)]
    public Vector3 Color;
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

    private DrawPipeline _pipeline;

    public Game(ILogger<Game> logger, IGraphicsDevice graphics, IShaderSupport shaderSupport) {
        _logger = logger;
        _graphics = graphics;
        _shaderSupport = shaderSupport;
        _loadedTcs = new TaskCompletionSource();
    }

    public async AnabasisCoroutine Load() {
        _logger.LogDebug("Game.Load");

        _pipeline = _graphics.CreateDrawPipeline(await _shaderSupport.CompileShaderAsync<Shader>());

        _pipeline.CreateVertexBuffer<VertexData>(6, LoadInstanceVertices());
        _pipeline.CreateVertexBuffer<InstanceData>(100, LoadInstanceTransforms());

        _loadedTcs.SetResult();
    }

    private InstanceData[] LoadInstanceTransforms() {
        InstanceData[] data = new InstanceData[100];
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

        return data;
    }

    private VertexData[] LoadInstanceVertices() {
        VertexData[] data = new VertexData[6];
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

        return data;
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