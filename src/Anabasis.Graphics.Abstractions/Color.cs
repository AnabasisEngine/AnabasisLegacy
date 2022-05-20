using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Anabasis.Graphics.Abstractions;

/// <remarks>
/// Field initialization order requires this to be a non-record struct to support not having a five-parameter constructor
/// </remarks>
[StructLayout(LayoutKind.Explicit)]
[DebuggerDisplay("{DebugDisplayString,nq}")]
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

    public Color(Vector4 vector4) : this((byte)(vector4.X * 255), (byte)(vector4.Y * 255), (byte)(vector4.Z * 255),
        (byte)(vector4.W * 255)) { }

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
    
    public void Deconstruct(out byte r, out byte g, out byte b)
    {
        r = R;
        g = G;
        b = B;
    }
    
    public void Deconstruct(out float r, out float g, out float b)
    {
        r = R / 255f;
        g = G / 255f;
        b = B / 255f;
    }
    
    public void Deconstruct(out byte r, out byte g, out byte b, out byte a) {
        r = R;
        g = G;
        b = B;
        a = A;
    }
    
    public void Deconstruct(out float r, out float g, out float b, out float a)
    {
        r = R / 255f;
        g = G / 255f;
        b = B / 255f;
        a = A / 255f;
    }

    public bool Equals(Color other) => PackedValue == other.PackedValue;

    public override bool Equals(object? obj) => obj is Color other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(PackedValue);

    public static bool operator ==(Color left, Color right) => left.Equals(right);

    public static bool operator !=(Color left, Color right) => !(left == right);

    internal string DebugDisplayString => $"{R}  {G}  {B}  {A}";

    public Vector4 ToVector4() => new(R / 255f, G / 255f, B / 255f, A / 255f);

    public override string ToString() => $"(R:{R},G:{G},B:{B},A:{A}";

    public string ToHexString() => $"{R:X2}{G:X2}{B:X2}{A:X2}";
}