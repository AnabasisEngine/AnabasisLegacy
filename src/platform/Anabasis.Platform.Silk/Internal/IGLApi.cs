using System.Numerics;
using System.Runtime.CompilerServices;
using Anabasis.Platform.Silk.Buffers;
using Anabasis.Platform.Silk.Shader;
using Anabasis.Platform.Silk.Textures;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

/// <summary>
/// A wrapper over <see cref="GL"/> with the aim of using value objects to remove primitive obsession issues.
/// Set up as an interface to allow mocking for unit testing, in practice should always be an instance of <see cref="GlApi"/>
/// </summary>
public partial interface IGlApi : IDisposable
{
    void ObjectLabel<TName>(TName name, string label)
        where TName : struct, IGlHandle;

    string GetObjectLabel<TName>(TName name)
        where TName : struct, IGlHandle;

    void GetAndThrowError([CallerMemberName] string caller = null!);
}