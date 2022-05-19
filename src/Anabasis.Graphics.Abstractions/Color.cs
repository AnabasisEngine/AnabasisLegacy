using System.Runtime.InteropServices;

namespace Anabasis.Graphics.Abstractions;

/// <remarks>
/// Field initialization order requires this to be a non-record struct to support not having a five-parameter constructor
/// </remarks>
[StructLayout(LayoutKind.Explicit)]
public readonly partial struct Color : IEquatable<Color>
{
    public Color(byte r, byte g, byte b, byte a) {
        PackedValue = 0;
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public Color(uint packedValue) {
        R = 0;
        G = 0;
        B = 0;
        A = 0;
        PackedValue = packedValue;
    }

    [field: FieldOffset(0)]
    public readonly uint PackedValue;

    [field: FieldOffset(0)]
    public readonly byte R;

    [field: FieldOffset(1)]
    public readonly byte G;

    [field: FieldOffset(2)]
    public readonly byte B;

    [field: FieldOffset(3)]
    public readonly byte A;

    public void Deconstruct(out byte r, out byte g, out byte b, out byte a) {
        r = R;
        g = G;
        b = B;
        a = A;
    }

    public bool Equals(Color other) => PackedValue == other.PackedValue;

    public override bool Equals(object? obj) => obj is Color other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(PackedValue);

    public static bool operator ==(Color left, Color right) => left.Equals(right);

    public static bool operator !=(Color left, Color right) => !(left == right);
}