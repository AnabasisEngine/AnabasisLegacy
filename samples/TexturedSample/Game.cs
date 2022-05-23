using System.Numerics;
using Anabasis.Abstractions;
using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Images.Abstractions;
using Anabasis.Threading;
using Microsoft.Extensions.Logging;

namespace TexturedSample;

public class Game : IAnabasisGame
{
    [VertexType]
    public struct VertexData
    {
        [VertexAttribute("vPos")]
        public Vector3 Position;

        [VertexAttribute("vUv")]
        public Vector2 TexCoord;
    }

    private readonly TaskCompletionSource _loadTcs = new();
    private readonly ILogger<Game>        _logger;
    private readonly IGraphicsDevice      _graphics;
    private readonly IShaderSupport       _shaderSupport;
    private readonly IImageDataLoader     _imageLoader;
    private readonly ITextureSupport      _textureSupport;
    private          DrawPipeline         _pipeline = null!;
    private          ITexture2D           _texture = null!;
    private          Shader               _shader = null!;

    public Game(ILogger<Game> logger, IGraphicsDevice graphics, IShaderSupport shaderSupport,
        IImageDataLoader imageLoader, ITextureSupport textureSupport) {
        _logger = logger;
        _graphics = graphics;
        _shaderSupport = shaderSupport;
        _imageLoader = imageLoader;
        _textureSupport = textureSupport;
    }

    public async AnabasisCoroutine Load() {
        _logger.LogDebug("Game.Load");

        _shader = await _shaderSupport.CompileShaderAsync<Shader>();
        _pipeline = _graphics.CreateDrawPipeline(_shader);

        _pipeline.CreateVertexBuffer<VertexData>(4, new[] {
            new VertexData {
                Position = new Vector3(0.5f, 0.5f, 0f),
                TexCoord = new Vector2(1f, 0f),
            },
            new VertexData {
                Position = new Vector3(0.5f, -0.5f, 0f),
                TexCoord = new Vector2(1f, 1f),
            },

            new VertexData {
                Position = new Vector3(-0.5f, -0.5f, 0f),
                TexCoord = new Vector2(0f, 1f),
            },
            new VertexData {
                Position = new Vector3(-0.5f, 0.5f, 0f),
                TexCoord = new Vector2(0f, 0f),
            },
        });

        _pipeline.CreateIndexBuffer<ushort>(6, new ushort[] { 0, 1, 3, 1, 2, 3, });

        await using (FileStream stream = File.OpenRead("silk.png"))
            _texture = await _imageLoader.LoadAsync(_textureSupport, stream);

        _loadTcs.SetResult();
    }

    public void Update() { }

    public void Render() {
        if (!_loadTcs.Task.IsCompleted)
            return;
        _graphics.Clear(Color.Black, ClearFlags.Color | ClearFlags.Depth);
        ITextureBinding binding = _texture.Bind(0);
        _shader.TextureUniform.Value = binding;
        _pipeline.DrawElements(DrawMode.Triangles, 6, 0);
    }
}