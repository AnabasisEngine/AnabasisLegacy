namespace Anabasis.Core;

public interface IAnabasisHandle<TApi>
{

    public void Free(TApi api);

    public uint Value { get; }
}