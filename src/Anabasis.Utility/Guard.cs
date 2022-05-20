using System.Runtime.CompilerServices;

namespace Anabasis.Utility;

public static class Guard
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsType<T>(object from, string? message = null,
        [CallerArgumentExpression("from")] string argName = null!) =>
        from is not T t ? throw new ArgumentException(message, argName) : t;
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TTo IsType<TFrom, TTo>(ref TFrom from, string? message = null,
        [CallerArgumentExpression("from")] string argName = null!) {
        if (from is not TTo)
            throw new ArgumentException(message, argName);
        return ref Unsafe.As<TFrom, TTo>(ref from);
    }
}