namespace Anabasis.Platform.Abstractions.Resources;

public interface IPlatformResource : IDisposable
{
    public IPlatformHandle Handle { get; }
    public void Use();
}