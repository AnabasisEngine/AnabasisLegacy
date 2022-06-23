using System.Numerics;

namespace Anabasis.Core;

public static class MiscMath
{
    public static T Align<T>(T value, T alignment)
        where T : INumber<T> =>
        value % alignment == T.Zero ? value : value + (alignment - value % alignment);
}