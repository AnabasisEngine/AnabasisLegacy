using System.Numerics;
using Anabasis.Platform.Silk.Internal;

namespace Anabasis.Platform.Silk.Shader.Parameters;

internal class Matrix4Parameter : SilkShaderParameter<Matrix4x4>
{
    public Matrix4Parameter(IGlApi gl, string name, ProgramHandle program) : base(gl, name, program) { }
    protected override void SetValue(uint programHandle, int location, in Matrix4x4 value) => Gl.ProgramUniformMatrix4(programHandle, location, false, value);
}