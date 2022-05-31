namespace Anabasis.Core;

public class MiscMath
{
    public static uint Align(uint value, uint alignment) =>
        value % alignment == 0 ? value : value + (alignment - value % alignment);

    public static int Align(int value, int alignment) =>
        value % alignment == 0 ? value : value + (alignment - value % alignment);
}