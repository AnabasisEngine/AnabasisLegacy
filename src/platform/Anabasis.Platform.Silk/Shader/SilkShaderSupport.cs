using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Platform.Abstractions;
using Anabasis.Platform.Silk.Internal;
using Anabasis.Platform.Silk.Shader.Parameters;
using Anabasis.Threading;
using Anabasis.Utility;
using Silk.NET.OpenGL;
using GlShaderType = Silk.NET.OpenGL.ShaderType;
using ShaderType = Anabasis.Graphics.Abstractions.Shaders.ShaderType;

namespace Anabasis.Platform.Silk.Shader;

internal partial class SilkShaderSupport : IShaderSupport
{
    private readonly ParameterConstructorProvider _parameterProvider;
    private readonly AnabasisTaskManager          _taskManager;
    private readonly IGlApi                       _gl;

    public SilkShaderSupport(ParameterConstructorProvider parameterProvider, IGraphicsDevice graphicsDevice,
        AnabasisTaskManager taskManager) {
        _parameterProvider = parameterProvider;
        _taskManager = taskManager;
        _gl = Guard.IsType<SilkGraphicsDevice>(graphicsDevice).Gl;
    }

    public async ValueTask<IPlatformHandle> CompileAndLinkAsync(IShaderProgramTexts texts,
        CancellationToken cancellationToken = default) {
        (ShaderType, Task<string>)[] t = texts.GetTexts().ToArray();
        ShaderHandle[] shaders = new ShaderHandle[t.Length];
        ProgramHandle program;
        try {
            int i = 0;
            foreach ((ShaderType shaderType, Task<string>? task) in t) {
                cancellationToken.ThrowIfCancellationRequested();
                GlShaderType glShaderType = ShaderTypeToNative(shaderType);
                string strings = await task;
                
                await _taskManager.Yield();
                
                ShaderHandle handle = _gl.CreateShader(glShaderType);
                _gl.ShaderSource(handle, strings);
                _gl.CompileShader(handle);
                _gl.GetShader(handle, ShaderParameterName.CompileStatus, out int isCompiled);
                if (isCompiled == 0) {
                    throw new Exception(
                        $"Error compiling shader of type {shaderType}, failed with error {_gl.GetShaderInfoLog(handle)}");
                }

                shaders[i++] = handle;
            }

            program = _gl.CreateProgram();
            foreach (ShaderHandle shader in shaders) {
                _gl.AttachShader(program, shader);
            }

            _gl.LinkProgram(program);
            _gl.GetProgram(program, ProgramPropertyARB.LinkStatus, out int status);
            if (status == 0) {
                throw new Exception($"Program failed to link with error: {_gl.GetProgramInfoLog(program)}");
            }

            foreach (ShaderHandle shader in shaders) {
                _gl.DetachShader(program, shader);
            }
        }
        finally {
            foreach (ShaderHandle shader in shaders) {
                _gl.DeleteShader(shader);
            }
        }

        return program;
    }

    public IShaderParameter<TParam> CreateParameter<TParam>(string name, IPlatformHandle programHandle) {
        ProgramHandle program = Guard.IsType<ProgramHandle>(programHandle);
        return _parameterProvider.Get<TParam>().Create(_gl, name, program);
    }

    private static GlShaderType ShaderTypeToNative(ShaderType type) => type switch {
        ShaderType.Fragment => GlShaderType.FragmentShader,
        ShaderType.Vertex => GlShaderType.VertexShader,
        ShaderType.Geometry => GlShaderType.GeometryShader,
        ShaderType.TessEval => GlShaderType.TessEvaluationShader,
        ShaderType.TessControl => GlShaderType.TessControlShader,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    public void UseShaderProgram(IPlatformHandle program) {
        _gl.UseProgram(Guard.IsType<ProgramHandle>(program));
    }

    public void DisposeProgram(IPlatformHandle program) {
        _gl.DeleteProgram(Guard.IsType<ProgramHandle>(program));
    }

    public IPlatformHandle NullHandle { get; } = new ProgramHandle(0);
}