using System.Runtime.InteropServices;
using Silk.NET.Maths;

namespace Anabasis.Core;

public static class MathExtensions
{
    public static Span<T> AsSpan<T>(this Vector2D<T> vector)
        where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T> => AsSpanInternal<Vector2D<T>, T>(vector);

    public static Span<T> AsSpan<T>(this Vector3D<T> vector)
        where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T> => AsSpanInternal<Vector3D<T>, T>(vector);

    public static Span<T> AsSpan<T>(this Vector4D<T> vector)
        where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T> => AsSpanInternal<Vector4D<T>, T>(vector);

    public static Span<T> AsSpan<T>(this Matrix4X4<T> vector)
        where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T> => AsSpanInternal<Matrix4X4<T>, T>(vector);
    public static Span<T> AsSpan<T>(this Rectangle<T> vector)
        where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T> => AsSpanInternal<Rectangle<T>, T>(vector);

    private static Span<TValue> AsSpanInternal<T, TValue>(T obj)
        where T : struct
        where TValue : struct => MemoryMarshal.Cast<T, TValue>(MemoryMarshal.CreateSpan(ref obj, 1));
}