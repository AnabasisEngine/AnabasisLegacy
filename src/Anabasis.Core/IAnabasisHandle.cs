using Silk.NET.OpenGL;

namespace Anabasis.Core;

public interface IAnabasisHandle
{
    public static abstract ObjectIdentifier ObjectType { get; }

    public void Free(GL gl);

    public uint Value { get; }
}

public interface IAnabasisBindableHandle : IAnabasisHandle
{
    public void Use(GL gl);
}