using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk;

public static class SilkLogIds
{
    public const int GlDebugProc = 1;
}

public static partial class SilkLog
{
    [LoggerMessage(EventId = SilkLogIds.GlDebugProc, Message = "OpenGl debug message: {Source}, {Type}, {Id}: {Message}")]
    public static partial void GlMessage(this ILogger logger, LogLevel level, DebugSource source, DebugType type, int id, string? message);
}