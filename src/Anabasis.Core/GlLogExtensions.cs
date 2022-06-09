using Silk.NET.Core.Native;
using Silk.NET.OpenGL;

namespace Anabasis.Core;

public static class GlLogExtensions
{
    public static DebugProc CreateDebugProc(Action<DebugSource, DebugType, DebugSeverity, int, string?> handler) =>
        (sourceEnum, typeEnum, id, severityEnum, length, messagePtr, param) => {
            DebugSource source = (DebugSource)sourceEnum;
            DebugType type = (DebugType)typeEnum;
            DebugSeverity severity = (DebugSeverity)severityEnum;
            string? message = SilkMarshal.PtrToString(messagePtr);
            handler(source, type, severity, id, message);
        };
}