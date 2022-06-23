using Anabasis.Core;
using Anabasis.Core.Graphics;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;

namespace Anabasis.Hosting.Hosting;

public sealed class AnabasisLoggingAdapter : IDisposable
{
    private readonly ILogger _logger;
    private readonly GL      _gl;

    public unsafe AnabasisLoggingAdapter(ILoggerFactory logger, GL gl) {
        _logger = logger.CreateLogger("OpenGL");
        _gl = gl;
        _gl.Enable(EnableCap.DebugOutput);
        _gl.Enable(EnableCap.DebugOutputSynchronous);
        _gl.DebugMessageCallback(GlLogExtensions.CreateDebugProc(Handler), null);
    }

    private void Handler(DebugSource source, DebugType type, DebugSeverity severity, int id, string? message) {
        LogLevel level = severity switch {
            DebugSeverity.DebugSeverityNotification => LogLevel.Debug,
            DebugSeverity.DebugSeverityHigh => throw new GlDebugException(source, type, severity, id, message),
            DebugSeverity.DebugSeverityMedium => LogLevel.Warning,
            DebugSeverity.DebugSeverityLow => LogLevel.Information,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null),
        };
        _logger.GlMessage(level, source, type, id, message);
    }

    public unsafe void Dispose() {
        _gl.DebugMessageCallback(null, null);
    }
}

internal static class SilkLogIds
{
    public const int GlDebugProc = 1;
}

internal static partial class AnabasisLog
{
    [LoggerMessage(EventId = SilkLogIds.GlDebugProc,
        Message = "OpenGl debug message: {Source}, {Type}, {Id}: {Message}")]
    public static partial void GlMessage(this ILogger logger, LogLevel level, DebugSource source, DebugType type,
        int id, string? message);
}