using System.Numerics;
using Anabasis.Core;
using FluentAssertions;

namespace BasicTests;

public class MathTests
{
#pragma warning disable xUnit1010
    [Theory]
    [InlineData(1u, 4u, 4u)]
    [InlineData(0u, 4u, 0u)]
    [InlineData(1, 4, 4)]
    [InlineData(0, 4, 0)]
    public void AlignmentTests<T>(T value, T alignment, T expected)
        where T : INumber<T> {
        MiscMath.Align(value, alignment).Should().Be(expected);
    }
#pragma warning restore xUnit1010
}