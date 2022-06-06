using Silk.NET.OpenGL;

namespace Anabasis.Core.Handles;

public record struct PipelineHandle(uint Value) : IAnabasisBindableHandle
{
    public static ObjectIdentifier ObjectType => ObjectIdentifier.ProgramPipeline;
    public void Free(GL gl) {
        gl.DeleteProgramPipeline(Value);
    }

    public void Use(GL gl) {
        gl.BindProgramPipeline(Value);
    }
}