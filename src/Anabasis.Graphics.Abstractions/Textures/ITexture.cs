using Anabasis.Platform.Abstractions.Resources;

namespace Anabasis.Graphics.Abstractions.Textures;

public interface ITexture<out TBinding> : IPlatformResource
where TBinding : struct
{
    public TBinding Bind(int unit);
}