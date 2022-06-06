using Silk.NET.OpenGL;

namespace Anabasis.Core.Handles;

public record struct ShaderHandle(uint Value) : IAnabasisHandle
{
    public static ObjectIdentifier ObjectType => ObjectIdentifier.Shader;
    public void Free(GL gl) {
        gl.DeleteShader(Value);
    }
}