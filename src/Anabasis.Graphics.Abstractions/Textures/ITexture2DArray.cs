namespace Anabasis.Graphics.Abstractions.Textures;

public interface ITexture2DArray : ITextureArray, ITexture3D
{
    public ITextureView2D Upload(int layer);
}