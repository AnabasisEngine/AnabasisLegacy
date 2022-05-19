using System.Numerics;
using System.Runtime.InteropServices;
using Anabasis.Platform.Silk.Shader;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

internal partial class GlApi
{
    public ShaderHandle CreateShader(ShaderType glShaderType) => new(_gl.CreateShader(glShaderType));

    public void ShaderSource(ShaderHandle handle, string[] strings) {
        _gl.ShaderSource(handle.Value, (uint)strings.Length, strings, 0);
    }

    public void CompileShader(ShaderHandle handle) {
        _gl.CompileShader(handle.Value);
    }

    public void GetShader(ShaderHandle handle, ShaderParameterName parameterName, out int i) {
        _gl.GetShader(handle.Value, parameterName, out i);
    }

    public string GetShaderInfoLog(ShaderHandle handle) => _gl.GetShaderInfoLog(handle.Value);
    public ProgramHandle CreateProgram() => new(_gl.CreateProgram());

    public void AttachShader(ProgramHandle program, ShaderHandle shader) {
        _gl.AttachShader(program.Value, shader.Value);
    }

    public void LinkProgram(ProgramHandle program) {
        _gl.LinkProgram(program.Value);
    }

    public void GetProgram(ProgramHandle program, ProgramPropertyARB programProperty, out int i) {
        _gl.GetProgram(program.Value, programProperty, out i);
    }

    public string GetProgramInfoLog(ProgramHandle program) => _gl.GetProgramInfoLog(program.Value);

    public void DetachShader(ProgramHandle program, ShaderHandle shader) {
        _gl.DetachShader(program.Value, shader.Value);
    }

    public void DeleteShader(ShaderHandle shader) {
        _gl.DeleteShader(shader.Value);
    }

    public int GetAttribLocation(ProgramHandle program, string name) => _gl.GetAttribLocation(program.Value, name);

    public int UniformLocation(ProgramHandle program, string name) =>
        _gl.GetUniformLocation(program.Value, name);

    public void ProgramUniformMatrix4(uint programHandle, int location, bool transpose, Matrix4x4 matrix) {
        _gl.ProgramUniformMatrix4(programHandle, location, transpose,
            MemoryMarshal.Cast<Matrix4x4, float>(MemoryMarshal.CreateSpan(ref matrix, 1)));
    }

    public void ProgramUniform(uint programHandle, int location, int value) {
        _gl.ProgramUniform1(programHandle, location, value);
    }
}