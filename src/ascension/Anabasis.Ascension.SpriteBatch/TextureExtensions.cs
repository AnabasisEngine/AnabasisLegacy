using System.Numerics;
using Anabasis.Graphics.Abstractions.Textures;
using Silk.NET.Maths;

namespace Anabasis.Ascension.SpriteBatch;

internal static class TextureExtensions
{
    internal static Vector2 TexelCoeffs(this ITextureView2D texture) => Vector2.One / texture.Size.As<float>().ToSystem();

    internal static Vector2D<ushort> NormalizeTexCoord(this ITextureView2D texture, Vector2D<ushort> coords) =>
        (coords.As<float>().ToSystem() * texture.TexelCoeffs() * ushort.MaxValue).ToGeneric().As<ushort>();
}