using Silk.NET.OpenGL;

namespace Anabasis.Core;

public interface IKindedHandle
{
    public static abstract ObjectIdentifier ObjectType { get; }
}