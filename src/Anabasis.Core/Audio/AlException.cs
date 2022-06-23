namespace Anabasis.Core.Audio;

public class AlException : Exception
{
    public AlException(AlError errorCode, string function, string message) : base(message) {
        ErrorCode = errorCode;
        Function = function;
    }

    public AlException(string? message) : base(message) { }
    
    public AlException(AlError errorCode, string function, string message, Exception innerException) : base(message, innerException) {
        ErrorCode = errorCode;
        Function = function;
    }

    public AlException(string? message, Exception? innerException) : base(message, innerException) { }
    public AlError ErrorCode { get; }
    public string Function { get; } = "";

    public override string Message {
        get {
            string s = base.Message;
            if (ErrorCode != default && !string.IsNullOrWhiteSpace(Function)) {
                return $"{s}{Environment.NewLine}OpenAL Error caught with error code {ErrorCode} in {Function}";
            }

            if (ErrorCode != default) {
                return $"{s}{Environment.NewLine}OpenAL Error caught with error code {ErrorCode}";
            }

            if (!string.IsNullOrWhiteSpace(Function)) {
                return $"{s}{Environment.NewLine}OpenAL Error caught in {Function}";
            }

            return s;
        }
    }
}