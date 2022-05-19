using Anabasis.Platform.Abstractions;

namespace Anabasis.Graphics.Abstractions.Shaders;

public interface IShaderParameter<TParam>
{
    public string Name { get; }
    public IPlatformHandle Program { get; }
    public TParam Value { get; set; }
}