namespace Anabasis.Core;

public interface IAnabasisBindableObject<TApi> : IAnabasisNativeObject<TApi>
{
    public IDisposable Use();
}