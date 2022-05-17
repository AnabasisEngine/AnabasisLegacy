using Anabasis.Platform.Abstractions;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Buffers;

public readonly record struct VertexArrayHandle(uint Value) : IPlatformHandle, IGlHandle
{
    public static ObjectIdentifier ObjectType => ObjectIdentifier.VertexArray;
}