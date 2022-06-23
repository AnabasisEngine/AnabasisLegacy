using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics;

public class GlException : Exception
{
    public GlException(ErrorCode errorCode, string function, string message) : base(message) {
        ErrorCode = errorCode;
        Function = function;
    }

    public GlException(string? message) : base(message) { }
    
    public GlException(ErrorCode errorCode, string function, string message, Exception innerException) : base(message, innerException) {
        ErrorCode = errorCode;
        Function = function;
    }

    public GlException(string? message, Exception? innerException) : base(message, innerException) { }
    public ErrorCode ErrorCode { get; }
    public string Function { get; } = "";

    public override string Message {
        get {
            string s = base.Message;
            if (ErrorCode != default && !string.IsNullOrWhiteSpace(Function)) {
                return $"{s}{Environment.NewLine}OpenGL Error caught with error code {ErrorCode} in {Function}";
            } else if (ErrorCode != default) {
                return $"{s}{Environment.NewLine}OpenGL Error caught with error code {ErrorCode}";
            } else if (!string.IsNullOrWhiteSpace(Function)) {
                return $"{s}{Environment.NewLine}OpenGL Error caught in {Function}";
            }

            return s;
        }
    }
}