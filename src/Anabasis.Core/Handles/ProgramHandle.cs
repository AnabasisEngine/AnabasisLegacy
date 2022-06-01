using Silk.NET.OpenGL;

namespace Anabasis.Core.Handles;

public readonly record struct ProgramHandle(uint Value) : IAnabasisBindableHandle
{
    public static ObjectIdentifier ObjectType => ObjectIdentifier.Program;
    public void Free(GL gl) {
        gl.DeleteProgram(Value);
    }

    public void Use(GL gl) {
        gl.UseProgram(Value);
    }
}