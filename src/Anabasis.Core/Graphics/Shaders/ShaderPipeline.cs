using Anabasis.Core.Graphics.Handles;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Shaders;

public sealed class ShaderPipeline : AnabasisBindableGraphicsObject<PipelineHandle>, IShaderPackage
{
    public ShaderPipeline(GL api) : base(api, new PipelineHandle(api.CreateProgramPipeline())) { }

    public void AttachProgram(UseProgramStageMask stages, ShaderProgram program) {
        Api.UseProgramStages(Handle.Value, stages, program.Handle.Value);
        Api.ThrowIfError(nameof(Api.UseProgramStages));
    }
}