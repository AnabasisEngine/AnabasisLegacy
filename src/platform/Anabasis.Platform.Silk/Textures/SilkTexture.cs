using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Platform.Silk.Internal;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Textures;

public abstract class SilkTexture : SilkGlObject<TextureHandle>, ITexture
{
    internal SilkTexture(IGlApi glApi, TextureTarget target) : this(glApi, glApi.CreateTexture(target)) { }

    internal SilkTexture(IGlApi glApi, TextureHandle handle) : base(glApi, handle) { }
    public SizedInternalFormat Format { get; protected internal init; }
    public int MipmapLevels { get; protected internal init; }

    public void GenerateMipmap() {
        Gl.GenerateTextureMipmap(Handle);
    }

    public ITextureBinding Bind(int unit) {
        Gl.BindTextureUnit((uint)unit, Handle);
        return new TextureBinding(unit, Handle);
    }

    public override void Dispose() {
        GC.SuppressFinalize(this);
        Gl.DeleteTexture(Handle);
    }

    public override IDisposable Use() {
        Bind(0);
        return new GenericDisposer(() => Gl.BindTextureUnit(0, new TextureHandle(0)));
    }

    protected static int ComputeMipmapDimension(int mipmap, int length) => length / (1 << mipmap);
}