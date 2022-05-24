namespace Anabasis.Graphics.Abstractions.Textures;

public interface ITextureArray : ITexture
{
    int Layers { get; }

    public ITexture CreateView(Range levels, Range layers);
}