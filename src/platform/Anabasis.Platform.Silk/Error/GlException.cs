
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Error;

public class GlException : Exception
{
    public GlException(ErrorCode errorCode, string caller, string? message) : base(message) {
        ErrorCode = errorCode;
        Function = caller;
    }
    public ErrorCode ErrorCode { get; }
    public string Function { get; }
}