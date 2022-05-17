using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Platform.Silk.Internal;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Textures;

public abstract class SilkTexture : SilkGlObject<TextureHandle>, ITexture<TextureBinding>
{
    private readonly IGlApi        _glApi;

    internal SilkTexture(IGlApi glApi, TextureTarget target) : base(glApi, glApi.CreateTexture(target)) {
        _glApi = glApi;
    }

    public TextureBinding Bind(int unit) {
        _glApi.BindTextureUnit((uint)unit, Handle);
        return new TextureBinding(unit, Handle);
    }

    public override void Dispose() {
        GC.SuppressFinalize(this);
        _glApi.DeleteTexture(Handle);
    }

    public override void Use() {
        Bind(0);
    }
}