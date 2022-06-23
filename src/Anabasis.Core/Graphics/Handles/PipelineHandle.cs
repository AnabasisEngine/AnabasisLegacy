using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Handles;

public record struct PipelineHandle(uint Value) : IAnabasisBindableHandle<GL>, IKindedHandle
{
    public static ObjectIdentifier ObjectType => ObjectIdentifier.ProgramPipeline;
    public void Free(GL api) {
        api.DeleteProgramPipeline(Value);
    }

    public void Use(GL api) {
        api.BindProgramPipeline(Value);
    }
}