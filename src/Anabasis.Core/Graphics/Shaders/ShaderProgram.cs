using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using Anabasis.Core.Graphics.Handles;
using Anabasis.Tasks;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Shaders;

public sealed partial class ShaderProgram : AnabasisBindableGraphicsObject<ProgramHandle>, IShaderPackage
{
    private ShaderProgram(GL api, ProgramHandle name) : base(api, name) { }

    public static async AnabasisTask<ShaderProgram> CreateSeparableShaderProgram(GL gl, ShaderType shaderType,
        params string[] source) {
        await AnabasisTask.SwitchToMainThread();
        uint program = gl.CreateShaderProgram(shaderType, 1, source);
        gl.ThrowIfError(nameof(gl.CreateShaderProgram));
        gl.GetProgram(program, ProgramPropertyARB.LinkStatus, out int status);
        if (status == 0) {
            throw new GlException($"Program failed to link with error: {gl.GetProgramInfoLog(program)}");
        }
        return new ShaderProgram(gl, new ProgramHandle(program));
    }

    public static Builder CreateBuilder(GL gl) => new(gl);

    public void SetUniform1(string name, ref int? location, float value) {
        Api.ProgramUniform1(Handle.Value, GetUniformLocation(name, ref location), value);
        Api.ThrowIfError(nameof(Api.ProgramUniform1));
    }

    private int GetUniformLocation(string name, [NotNull] ref int? location) {
        int uniformLocation = location ??= Api.GetUniformLocation(Handle.Value, name);
        Api.ThrowIfError(nameof(Api.GetUniformLocation));
        return uniformLocation;
    }

    public void SetUniform1(string name, ref int? location, int value) {
        Api.ProgramUniform1(Handle.Value, GetUniformLocation(name, ref location), value);
        Api.ThrowIfError(nameof(Api.ProgramUniform1));
    }

    public void SetUniformMatrix4(string name, ref int? location, bool transpose, Matrix4x4 matrix) {
        Api.ProgramUniformMatrix4(Handle.Value, GetUniformLocation(name, ref location), transpose,
            MemoryMarshal.Cast<Matrix4x4, float>(MemoryMarshal.CreateReadOnlySpan(ref matrix, 1)));
        Api.ThrowIfError(nameof(Api.ProgramUniformMatrix4));
    }

    public sealed class Builder : IDisposable
    {
        private readonly GL         _gl;
        private readonly List<uint> _shaders;

        internal Builder(GL gl) {
            _gl = gl;
            _shaders = new List<uint>();
        }

        public async AnabasisTask CompileAsync(ShaderType shaderType, params string[] source) {
            await AnabasisTask.SwitchToMainThread();
            uint shader = _gl.CreateShader(shaderType);
            _shaders.Add(shader);
            _gl.ShaderSource(shader, (uint)source.Length, source, 0);
            _gl.GetShader(shader, ShaderParameterName.CompileStatus, out int isCompiled);
            if (isCompiled == 0) {
                throw new GlException(
                    $"Error compiling shader of type {shaderType}, failed with error {_gl.GetShaderInfoLog(shader)}");
            }
        }

        public async AnabasisTask<ShaderProgram> LinkAsync() {
            await AnabasisTask.SwitchToMainThread();
            uint handle = _gl.CreateProgram();
            using(this) {
                foreach (uint shader in _shaders) {
                    _gl.AttachShader(handle, shader);
                }

                _gl.LinkProgram(handle);
                _gl.GetProgram(handle, ProgramPropertyARB.LinkStatus, out int status);
                if (status == 0) {
                    throw new GlException($"Program failed to link with error: {_gl.GetProgramInfoLog(handle)}");
                }

                foreach (uint shader in _shaders) {
                    _gl.DetachShader(handle, shader);
                }

                return new ShaderProgram(_gl, new ProgramHandle(handle));
            }
        }

        public void Dispose() {
            foreach (uint shader in _shaders) {
                _gl.DeleteShader(shader);
            }
        }
    }
}