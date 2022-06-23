using System.Numerics;
using Anabasis.Ascension;
using Anabasis.Core;
using Anabasis.Core.Graphics.Buffers;
using Anabasis.Core.Graphics.Rendering;
using Anabasis.Core.Graphics.Shaders;
using Anabasis.Core.Graphics.Textures;
using Anabasis.ImageSharp;
using Anabasis.Tasks;
using Silk.NET.OpenGL;
using Color = Anabasis.Core.Color;
using VertexArray = Anabasis.Core.Graphics.Rendering.VertexArray;

namespace AscensionSample;

public class Game : AscensionGame
{
    public Game(GL gl, AscensionSupport support) : base(gl, support) { }

    public override async AnabasisTask LoadAsync() {
        await base.LoadAsync();
        Gl.Enable(EnableCap.Blend);
        Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    protected override AnabasisTask<IAnabasisContext> CreateInitialSceneAsync() =>
        AnabasisTask.FromResult<IAnabasisContext>(CreateScene<StartingScene>());
}

public class StartingScene : IAscensionScene, IDisposable
{
    private readonly GL               _gl;
    private readonly ImageSharpLoader _imageLoader;
    private          Texture2D        _texture = null!;
    private          ShaderProgram    _vert    = null!;
    private          ShaderProgram    _frag    = null!;
    private readonly ShaderPipeline   _pipeline;
    private readonly VertexArray      _array;
    private readonly GraphicsBuffer   _buffer;
    private          int?             _texUniformLoc;

    public StartingScene(GL gl, ImageSharpLoader imageLoader) {
        _gl = gl;
        _imageLoader = imageLoader;
        _pipeline = new ShaderPipeline(_gl);
        _array = new VertexArray(_gl);
        _buffer = new GraphicsBuffer(_gl);
    }

    public void Update() { }

    public void Render() {
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _gl.ClearColor(Color.Wheat);
        using RenderBatch batch = RenderBatch.Begin(_gl, _pipeline, _array);
        batch.DrawElements(PrimitiveType.Triangles, 6, 0);
    }

    public async AnabasisTask LoadAsync(IProgress<SceneLoadStatus> progress) {
        _texture = await _imageLoader.LoadImageAsync("silk.png");

        _vert = await ShaderProgram.CreateSeparableShaderProgram(_gl, ShaderType.VertexShader,
            await File.ReadAllTextAsync("shader.vert"));
        _frag = await ShaderProgram.CreateSeparableShaderProgram(_gl, ShaderType.FragmentShader,
            await File.ReadAllTextAsync("shader.frag"));
        _pipeline.AttachProgram(UseProgramStageMask.VertexShaderBit, _vert);
        _pipeline.AttachProgram(UseProgramStageMask.FragmentShaderBit, _frag);

        _buffer.AllocateBuffer(32);
        _buffer.Typed<ushort>().Slice(0, 12).Write(new ushort[] { 0, 1, 3, 1, 2, 3, });
        _buffer.LoadData<Vertex>(span => {
            span[0].Position = new Vector3(0.5f, 0.5f, 0f);
            span[0].TexCoord = new Vector2(1f, 0f);

            span[1].Position = new Vector3(0.5f, -0.5f, 0f);
            span[1].TexCoord = new Vector2(1f, 1f);

            span[2].Position = new Vector3(-0.5f, -0.5f, 0f);
            span[2].TexCoord = new Vector2(0f, 1f);

            span[3].Position = new Vector3(-0.5f, 0.5f, 0f);
            span[3].TexCoord = new Vector2(0f, 0f);
        }, MiscMath.Align(12, 4), 16);

        _array.FormatAndBindVertexBuffer(_buffer.Slice<Vertex>(12, 16));
        _array.BindIndexBuffer(_buffer.Typed<ushort>());

        _texture.BindTo(0);
        _frag.SetUniform1("uTexture0", ref _texUniformLoc, 0);
    }

    public void Dispose() {
        _texture.Dispose();
    }
}