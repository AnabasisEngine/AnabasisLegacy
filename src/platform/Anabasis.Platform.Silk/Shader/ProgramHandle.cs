using Anabasis.Platform.Abstractions;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Shader;

public readonly record struct ProgramHandle(uint Value) : IPlatformHandle, IGlHandle
{
    public static ObjectIdentifier ObjectType => ObjectIdentifier.Program;
}