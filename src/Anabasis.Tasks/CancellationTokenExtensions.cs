namespace Anabasis.Tasks;

public static class CancellationTokenExtensions
{
    public static CancellationTokenRegistration RegisterWithoutCaptureExecutionContext(
        this CancellationToken cancellationToken, Action<object?> callback, object? state) {
        bool restoreFlow = false;
        if (!ExecutionContext.IsFlowSuppressed()) {
            ExecutionContext.SuppressFlow();
            restoreFlow = true;
        }

        try {
            return cancellationToken.Register(callback, state, false);
        }
        finally {
            if (restoreFlow) ExecutionContext.RestoreFlow();
        }
    }

    public static CancellationTokenRegistration RegisterOnGameLoop(this CancellationToken cancellationToken,
        Action<object?> callback, object? state) => RegisterWithoutCaptureExecutionContext(cancellationToken,
        _ => AnabasisTaskScheduler.Schedule(AnabasisPlatformStepMask.All, callback, state), null);
}