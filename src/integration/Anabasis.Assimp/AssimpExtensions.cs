using Silk.NET.Assimp;

namespace Anabasis.Assimp;

public static class AssimpExtensions
{
    public static void CollectError(this Return result, Silk.NET.Assimp.Assimp assimp) {
        switch (result) {
            case Return.ReturnSuccess:
                return;
            case Return.ReturnFailure:
                throw new AssimpException($"Assimp Error: {assimp.GetErrorStringS()}");
            case Return.ReturnOutofmemory:
                throw new OutOfMemoryException();
            default:
                throw new ArgumentOutOfRangeException(nameof(result), result, null);
        }
    }
}