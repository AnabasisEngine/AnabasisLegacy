using System.Diagnostics;
using System.Runtime.CompilerServices;
using Anabasis.Platform.Silk.Error;
using Microsoft.Extensions.Logging;
using Silk.NET.Core.Native;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

internal partial class GlApi : IGlApi
{
    private readonly GL             _gl;
    private readonly ILogger<GlApi> _logger;

    public unsafe GlApi(GL gl, ILogger<GlApi> logger) {
        _gl = gl;
        _logger = logger;
        _gl.Enable(EnableCap.DebugOutput);
        _gl.Enable(EnableCap.DebugOutputSynchronous);
        _gl.DebugMessageCallback(DebugCallback, null);
    }

    private void DebugCallback(GLEnum sourceEnum, GLEnum typeEnum, int id, GLEnum severityEnum, int length,
        nint messagePtr, nint userParamPtr) {
        DebugSource source = (DebugSource)sourceEnum;
        DebugType type = (DebugType)typeEnum;
        DebugSeverity severity = (DebugSeverity)severityEnum;
        string? message = SilkMarshal.PtrToString(messagePtr);
        LogLevel level = severity switch {
            DebugSeverity.DontCare => throw new NotImplementedException(),
            DebugSeverity.DebugSeverityNotification => LogLevel.Debug,
            DebugSeverity.DebugSeverityHigh => throw new GlDebugException(source, type, severity, id, message),
            DebugSeverity.DebugSeverityMedium => LogLevel.Warning,
            DebugSeverity.DebugSeverityLow => LogLevel.Information,
            _ => throw new ArgumentOutOfRangeException(nameof(severityEnum), severity, null),
        };
        _logger.GlMessage(level, source, type, id, message);
    }

    public void Dispose() {
        _gl.Dispose();
    }

    public void GetAndThrowError([CallerMemberName] string caller = null!) {
        ErrorCode error = (ErrorCode)_gl.GetError();
        if (error != ErrorCode.NoError)
            throw new GlException(error, caller, "GL Error caught with glGetError");
    }

    public void ObjectLabel<TName>(TName name, string label)
        where TName : struct, IGlHandle =>
        _gl.ObjectLabel(TName.ObjectType, name.Value, (uint)label.Length, label);

    public string GetObjectLabel<TName>(TName name)
        where TName : struct, IGlHandle {
        _gl.GetInteger(GetPName.MaxLabelLength, out int maxLength);
        _gl.GetObjectLabel(TName.ObjectType, name.Value, (uint)Math.Min(maxLength, 128), out uint _, out string label);
        return label;
    }

    public void MemoryBarrier(MemoryBarrierMask mask) {
        _gl.MemoryBarrier(mask);
    }

    public void FenceAndWait(uint timeoutNanoseconds, SyncBehaviorFlags flags = SyncBehaviorFlags.None) {
        nint fence = _gl.FenceSync(SyncCondition.SyncGpuCommandsComplete, flags);
        switch (_gl.ClientWaitSync(fence, SyncObjectMask.SyncFlushCommandsBit, timeoutNanoseconds)) {
            case GLEnum.TimeoutExpired:
                throw new TimeoutException();
            case GLEnum.WaitFailed:
                GetAndThrowError();
                throw new UnreachableException();
        }
    }
}