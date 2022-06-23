using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Handles;

public readonly record struct ProgramHandle(uint Value) : IAnabasisBindableHandle<GL>, IKindedHandle {
    public static ObjectIdentifier ObjectType => ObjectIdentifier.Program;
    public void Free(GL api) {
        api.DeleteProgram(Value);
    }

    public void Use(GL api) {
        api.UseProgram(Value);
    }
}