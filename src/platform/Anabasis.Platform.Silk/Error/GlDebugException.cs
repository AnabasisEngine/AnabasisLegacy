using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Error;

public class GlDebugException : Exception
{
    public DebugSource GlSource { get; }
    public DebugType GlType { get; }
    public DebugSeverity GlSeverity { get; }
    public int GlMessageId { get; }

    public GlDebugException(DebugSource source, DebugType type, DebugSeverity severity, int glMessageId,
        string? message) : base(message) {
        GlSource = source;
        GlType = type;
        GlSeverity = severity;
        GlMessageId = glMessageId;
    }

    public override string ToString() =>
        $"{base.ToString()}, {nameof(GlSource)}: {GlSource}, {nameof(GlType)}: {GlType}, {nameof(GlSeverity)}: {GlSeverity}, {nameof(GlMessageId)}: {GlMessageId}";
}