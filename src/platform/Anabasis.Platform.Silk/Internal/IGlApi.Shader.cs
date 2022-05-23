using System.Numerics;
using Anabasis.Platform.Silk.Shader;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

public partial interface IGlApi
{
    int GetAttribLocation(ProgramHandle program, string name);
    ShaderHandle CreateShader(ShaderType glShaderType);
    void ShaderSource(ShaderHandle handle, string @string);
    void CompileShader(ShaderHandle handle);
    void GetShader(ShaderHandle handle, ShaderParameterName parameterName, out int i);
    string GetShaderInfoLog(ShaderHandle handle);
    ProgramHandle CreateProgram();
    void AttachShader(ProgramHandle program, ShaderHandle shader);
    void LinkProgram(ProgramHandle program);
    void GetProgram(ProgramHandle program, ProgramPropertyARB programProperty, out int i);
    string GetProgramInfoLog(ProgramHandle program);
    void DetachShader(ProgramHandle program, ShaderHandle shader);
    void DeleteShader(ShaderHandle shader);
    int UniformLocation(ProgramHandle program, string name);
    void ProgramUniformMatrix4(uint programHandle, int location, bool transpose, Matrix4x4 matrix);
    void ProgramUniform(uint programHandle, int location, int value);
    void UseProgram(ProgramHandle program);
    void DeleteProgram(ProgramHandle program);
}