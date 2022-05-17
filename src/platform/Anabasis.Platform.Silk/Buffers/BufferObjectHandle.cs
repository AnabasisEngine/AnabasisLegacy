using Anabasis.Platform.Abstractions;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Buffers;

public readonly record struct BufferObjectHandle(uint Value) : IPlatformHandle, IGlHandle
{
    public static ObjectIdentifier ObjectType => ObjectIdentifier.Buffer;
}