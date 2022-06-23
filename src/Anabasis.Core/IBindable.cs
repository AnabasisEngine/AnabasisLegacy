namespace Anabasis.Core;

public interface IBindable<TApi>
{
    void Use(TApi api);
}