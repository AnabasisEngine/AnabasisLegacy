using System.Diagnostics;
using System.Runtime.CompilerServices;
using Anabasis.Platform.Silk.Error;
using Microsoft.Extensions.Logging;
using Silk.NET.Core.Native;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

internal partial class GlApi : IGlApi
{
    internal readonly GL             Gl;
    private readonly ILogger<GlApi> _logger;

    public unsafe GlApi(GL gl, ILogger<GlApi> logger) {
        Gl = gl;
        _logger = logger;
        Gl.Enable(EnableCap.DebugOutput);
        Gl.Enable(EnableCap.DebugOutputSynchronous);
        Gl.DebugMessageCallback(DebugCallback, null);
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
        Gl.Dispose();
    }

    public void GetAndThrowError([CallerMemberName] string caller = null!) {
        ErrorCode error = (ErrorCode)Gl.GetError();
        if (error != ErrorCode.NoError)
            throw new GlException(error, caller, "GL Error caught with glGetError");
    }

    public void ObjectLabel<TName>(TName name, string label)
        where TName : struct, IGlHandle =>
        Gl.ObjectLabel(TName.ObjectType, name.Value, (uint)label.Length, label);

    public string GetObjectLabel<TName>(TName name)
        where TName : struct, IGlHandle {
        Gl.GetInteger(GetPName.MaxLabelLength, out int maxLength);
        Gl.GetObjectLabel(TName.ObjectType, name.Value, (uint)Math.Min(maxLength, 128), out uint _, out string label);
        return label;
    }

    public void MemoryBarrier(MemoryBarrierMask mask) {
        Gl.MemoryBarrier(mask);
    }

    public void FenceAndWait(uint timeoutNanoseconds, SyncBehaviorFlags flags = SyncBehaviorFlags.None) {
        nint fence = Gl.FenceSync(SyncCondition.SyncGpuCommandsComplete, flags);
        switch (Gl.ClientWaitSync(fence, SyncObjectMask.SyncFlushCommandsBit, timeoutNanoseconds)) {
            case GLEnum.TimeoutExpired:
                throw new TimeoutException();
            case GLEnum.WaitFailed:
                GetAndThrowError();
                throw new UnreachableException();
        }
        Gl.DeleteSync(fence);
    }
}