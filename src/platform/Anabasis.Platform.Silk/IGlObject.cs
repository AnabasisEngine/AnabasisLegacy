namespace Anabasis.Platform.Silk;

public interface IGlObject
{
    public IGlHandle Handle { get; }

    public string Label { get; set; }
}

public interface IGlObject<out TName> : IGlObject
    where TName : struct, IGlHandle
{
    public new TName Handle { get; }
}