using Anabasis.Core.Handles;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Shaders;

public sealed class ShaderPipeline : AnabasisBindableNativeObject<PipelineHandle>, IShaderPackage
{
    public ShaderPipeline(GL gl) : base(gl, new PipelineHandle(gl.CreateProgramPipeline())) { }

    public void AttachProgram(UseProgramStageMask stages, ShaderProgram program) {
        Gl.UseProgramStages(Handle.Value, stages, program.Handle.Value);
        Gl.ThrowIfError(nameof(Gl.UseProgramStages));
    }
}