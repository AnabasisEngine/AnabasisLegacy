using System.Numerics;
using Anabasis.Ascension;
using Anabasis.Graphics.Abstractions.Textures;
using FluentAssertions;
using Moq;
using Silk.NET.Maths;

namespace SilkPlatformTests;

public class TextureNormalizationTests
{
    public static TheoryData<Vector2D<int>, Vector2D<ushort>, Vector2> NormalizationToFloatData = new() {
        { new Vector2D<int>(16, 16), new Vector2D<ushort>(0, 0), new Vector2(0f, 0f) },
        { new Vector2D<int>(16,16), new Vector2D<ushort>(16, 16), new Vector2(1f, 1f)},
    };

    [Theory, MemberData(nameof(NormalizationToFloatData))]
    public void NormalizationToFloat(Vector2D<int> size, Vector2D<ushort> pixelCoords, Vector2 normalizedCoords) {
        Mock<ITextureView2D> mock = new();
        mock.Setup(t => t.Size).Returns(size);
        mock.Object.NormalizeTexCoordFloat(pixelCoords).Should().Be(normalizedCoords);
    }
    
    public static TheoryData<Vector2D<int>, Vector2D<ushort>, Vector2D<ushort>> NormalizationToUshortData = new() {
        { new Vector2D<int>(16, 16), new Vector2D<ushort>(0, 0), new Vector2D<ushort>(0, 0) },
        { new Vector2D<int>(16,16), new Vector2D<ushort>(16, 16), new Vector2D<ushort>(65535, 65535)},
    };

    [Theory, MemberData(nameof(NormalizationToUshortData))]
    public void NormalizationToUshort(Vector2D<int> size, Vector2D<ushort> pixelCoords, Vector2D<ushort> normalizedCoords) {
        Mock<ITextureView2D> mock = new();
        mock.Setup(t => t.Size).Returns(size);
        mock.Object.NormalizeTexCoordUshort(pixelCoords).Should().Be(normalizedCoords);
    }
}