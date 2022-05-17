using Anabasis.Platform.Abstractions;

namespace Anabasis.Graphics.Abstractions.Shaders;

public interface IShaderParameter<TParam> where TParam : struct
{
    public string Name { get; }
    public IPlatformHandle Program { get; }
    public TParam Value { get; set; }
}