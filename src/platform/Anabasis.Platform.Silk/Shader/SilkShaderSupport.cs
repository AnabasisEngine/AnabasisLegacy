using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Platform.Abstractions;
using Anabasis.Platform.Silk.Internal;
using Anabasis.Utility;
using Silk.NET.OpenGL;
using GlShaderType = Silk.NET.OpenGL.ShaderType;
using ShaderType = Anabasis.Graphics.Abstractions.Shaders.ShaderType;

namespace Anabasis.Platform.Silk.Shader;

public partial class SilkShaderSupport : IShaderSupport
{
    public async ValueTask<IPlatformHandle> CompileAndLinkAsync(IGraphicsDevice provider, IShaderProgramTexts texts, CancellationToken cancellationToken) {
        IGlApi gl = Guard.IsType<SilkGraphicsDevice>(provider, "Unexpected platform implementation").Gl;
        ShaderHandle[] shaders = new ShaderHandle[texts.GetTexts().Count];
        ProgramHandle program;
        try {
            int i = 0;
            foreach (KeyValuePair<ShaderType, IAsyncEnumerable<string>> keyValuePair in texts.GetTexts()) {
                cancellationToken.ThrowIfCancellationRequested();
                GlShaderType glShaderType = ShaderTypeToNative(keyValuePair.Key);
                ShaderHandle handle = gl.CreateShader(glShaderType);
                string[] strings = await keyValuePair.Value.ToArrayAsync(cancellationToken);
                gl.ShaderSource(handle, strings);
                gl.CompileShader(handle);
                gl.GetShader(handle, ShaderParameterName.CompileStatus, out int isCompiled);
                if (isCompiled == 0) {
                    throw new Exception(
                        $"Error compiling shader of type {keyValuePair.Key}, failed with error {gl.GetShaderInfoLog(handle)}");
                }

                shaders[i++] = handle;
            }

            program = gl.CreateProgram();
            foreach (ShaderHandle shader in shaders) {
                gl.AttachShader(program, shader);
            }

            gl.LinkProgram(program);
            gl.GetProgram(program, ProgramPropertyARB.LinkStatus, out int status);
            if (status == 0) {
                throw new Exception($"Program failed to link with error: {gl.GetProgramInfoLog(program)}");
            }
            foreach (ShaderHandle shader in shaders) {
                gl.DetachShader(program, shader);
            }
        }
        finally {
            foreach (ShaderHandle shader in shaders) {
                gl.DeleteShader(shader);
            }
        }

        return program;
    }

    private static GlShaderType ShaderTypeToNative(ShaderType type) => type switch {
        ShaderType.Fragment => GlShaderType.FragmentShader,
        ShaderType.Vertex => GlShaderType.VertexShader,
        ShaderType.Geometry => GlShaderType.GeometryShader,
        ShaderType.TessEval => GlShaderType.TessEvaluationShader,
        ShaderType.TessControl => GlShaderType.TessControlShader,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}