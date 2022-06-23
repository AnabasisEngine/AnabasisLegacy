using Anabasis.Core.Graphics.Handles;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Textures;

public abstract class Texture : AnabasisGraphicsObject<TextureHandle>
{
    public TextureTarget Target { get; protected init; }
    public uint Mipmaps { get; }
    public SizedInternalFormat Format { get; }

    protected Texture(GL api, TextureHandle name, uint mipmaps, SizedInternalFormat format) :
        base(api, name) {
        Mipmaps = mipmaps;
        Format = format;
    }

    protected Texture(GL api, TextureTarget target, uint mipmaps, SizedInternalFormat format) : base(api,
        new TextureHandle(api.CreateTexture(target))) {
        Target = target;
        Mipmaps = mipmaps;
        Format = format;
    }

    public IDisposable BindTo(uint unit) {
        Api.BindTextureUnit(unit, Handle.Value);
        return new GenericDisposer(() => Api.BindTextureUnit(unit, 0));
    }

    public static void CopyPixels(Texture src, Texture dst, Box3D<int> srcRect, int srcLevel, Vector3D<int> dstOffset,
        int dstLevel) {
        src.Api.CopyImageSubData(src.Handle.Value, (CopyImageSubDataTarget)src.Target, srcLevel, srcRect.Min.X,
            srcRect.Min.Y, srcRect.Min.Z, dst.Handle.Value, (CopyImageSubDataTarget)dst.Target, dstLevel, dstOffset.X,
            dstOffset.Y, dstOffset.Z, (uint)srcRect.Size.X, (uint)srcRect.Size.Y, (uint)srcRect.Size.Z);
    }
}