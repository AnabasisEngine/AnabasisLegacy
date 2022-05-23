using System.Numerics;
using System.Runtime.InteropServices;
using Anabasis.Platform.Silk.Shader;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

internal partial class GlApi
{
    public ShaderHandle CreateShader(ShaderType glShaderType) => new(Gl.CreateShader(glShaderType));

    public void ShaderSource(ShaderHandle handle, string @string) {
        Gl.ShaderSource(handle.Value, @string);
    }

    public void CompileShader(ShaderHandle handle) {
        Gl.CompileShader(handle.Value);
    }

    public void GetShader(ShaderHandle handle, ShaderParameterName parameterName, out int i) {
        Gl.GetShader(handle.Value, parameterName, out i);
    }

    public string GetShaderInfoLog(ShaderHandle handle) => Gl.GetShaderInfoLog(handle.Value);
    public ProgramHandle CreateProgram() => new(Gl.CreateProgram());

    public void AttachShader(ProgramHandle program, ShaderHandle shader) {
        Gl.AttachShader(program.Value, shader.Value);
    }

    public void LinkProgram(ProgramHandle program) {
        Gl.LinkProgram(program.Value);
    }

    public void GetProgram(ProgramHandle program, ProgramPropertyARB programProperty, out int i) {
        Gl.GetProgram(program.Value, programProperty, out i);
    }

    public string GetProgramInfoLog(ProgramHandle program) => Gl.GetProgramInfoLog(program.Value);

    public void DetachShader(ProgramHandle program, ShaderHandle shader) {
        Gl.DetachShader(program.Value, shader.Value);
    }

    public void DeleteShader(ShaderHandle shader) {
        Gl.DeleteShader(shader.Value);
    }

    public int GetAttribLocation(ProgramHandle program, string name) => Gl.GetAttribLocation(program.Value, name);

    public int UniformLocation(ProgramHandle program, string name) =>
        Gl.GetUniformLocation(program.Value, name);

    public void ProgramUniformMatrix4(uint programHandle, int location, bool transpose, Matrix4x4 matrix) {
        Gl.ProgramUniformMatrix4(programHandle, location, transpose,
            MemoryMarshal.Cast<Matrix4x4, float>(MemoryMarshal.CreateSpan(ref matrix, 1)));
    }

    public void ProgramUniform(uint programHandle, int location, int value) {
        Gl.ProgramUniform1(programHandle, location, value);
    }

    public void UseProgram(ProgramHandle program) {
        Gl.UseProgram(program.Value);
    }

    public void DeleteProgram(ProgramHandle program) {
        Gl.DeleteProgram(program.Value);
    }
}