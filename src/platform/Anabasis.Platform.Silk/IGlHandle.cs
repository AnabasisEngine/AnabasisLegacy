using Anabasis.Platform.Abstractions;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk;

public interface IGlHandle : IPlatformHandle {
    public static abstract ObjectIdentifier ObjectType { get; }
    public uint Value { get; }
}