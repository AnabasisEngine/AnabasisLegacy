namespace Anabasis.Core;

public interface IAnabasisBindableObject : IAnabasisNativeObject
{
    public IDisposable Use();
}