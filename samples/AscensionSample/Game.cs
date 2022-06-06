using System.Numerics;
using Anabasis.Ascension;
using Anabasis.Core;
using Anabasis.Core.Buffers;
using Anabasis.Core.Rendering;
using Anabasis.Core.Shaders;
using Anabasis.Core.Textures;
using Anabasis.ImageSharp;
using Anabasis.Tasks;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = System.Drawing.Color;
using VertexArray = Anabasis.Core.Rendering.VertexArray;

namespace AscensionSample;

public class Game : AscensionGame
{
    public Game(GL gl) : base(gl) { }

    protected override AnabasisTask<IAnabasisContext> CreateInitialSceneAsync() =>
        AnabasisTask.FromResult<IAnabasisContext>(new StartingScene(Gl));
}

public class StartingScene : IAscensionScene, IDisposable
{
    private readonly GL             _gl;
    private          Texture2D      _texture = null!;
    private          ShaderProgram  _vert    = null!;
    private          ShaderProgram  _frag    = null!;
    private readonly ShaderPipeline _pipeline;
    private readonly VertexArray    _array;
    private readonly GraphicsBuffer _buffer;
    private          int?            _texUniformLoc;

    public StartingScene(GL gl) {
        _gl = gl;
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
        Configuration configuration = Configuration.Default.Clone();
        configuration.PreferContiguousImageBuffers = true;
        using (Image image = await Image.LoadAsync(configuration, "silk.png")) {
            await AnabasisTask.SwitchToMainThread();
            _texture = new Texture2D(_gl, (uint)image.Width, (uint)image.Height, 1, SizedInternalFormat.Rgba8);
            ImageSharpLoader.UploadImage(image, _texture);
        }

        _vert = await ShaderProgram.CreateSeparableShaderProgram(_gl, ShaderType.VertexShader,
            await File.ReadAllTextAsync("shader.vert"));
        _frag = await ShaderProgram.CreateSeparableShaderProgram(_gl, ShaderType.FragmentShader,
            await File.ReadAllTextAsync("shader.frag"));
        _pipeline.AttachProgram(UseProgramStageMask.VertexShaderBit, _vert);
        _pipeline.AttachProgram(UseProgramStageMask.FragmentShaderBit, _frag);

        _buffer.AllocateBuffer(28);
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
        }, 12, 16);
        
        _array.FormatAndBindVertexBuffer(_buffer.Slice<Vertex>(12, 16));
        _array.BindIndexBuffer(_buffer.Typed<ushort>());

        _texture.BindTo(0);
        _frag.SetUniform1("uTexture0", ref _texUniformLoc, 0);
    }

    public void Dispose() {
        _texture.Dispose();
    }
}