using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Anabasis.Tasks.CompilerServices;

namespace Anabasis.Tasks;

[AsyncMethodBuilder(typeof(AnabasisTaskAsyncMethodBuilder<>))]
[StructLayout(LayoutKind.Auto)]
public readonly struct AnabasisTask<T>
{
    private readonly IAnabasisTaskSource<T>? _source;
    readonly         T                       _result;
    private readonly short                   _token;

    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AnabasisTask(T result) {
        _source = default;
        _token = default;
        _result = result;
    }

    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AnabasisTask(IAnabasisTaskSource<T> source, short token) {
        _source = source;
        _token = token;
        _result = default!;
    }

    public AnabasisTaskStatus Status {
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            if (_source == null) return AnabasisTaskStatus.Succeeded;
            return _source.GetStatus(_token);
        }
    }

    public AnabasisTask AsAnabasisTask() {
        if (_source == null) return AnabasisTask.CompletedTask;

        AnabasisTaskStatus status = _source.GetStatus(_token);
        if (!status.IsCompletedSuccessfully()) return new AnabasisTask(_source, _token);
        _source.GetResult(_token);
        return AnabasisTask.CompletedTask;

        // Converting UniTask<T> -> UniTask is zero overhead.
    }

    public static implicit operator AnabasisTask(AnabasisTask<T> self) => self.AsAnabasisTask();

    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Awaiter GetAwaiter() => new Awaiter(this);

    public readonly struct Awaiter : ICriticalNotifyCompletion
    {
        readonly AnabasisTask<T> _task;

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Awaiter(in AnabasisTask<T> task) {
            _task = task;
        }

        public bool IsCompleted {
            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _task.Status.IsCompleted();
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetResult() => _task._source is { } s ? s.GetResult(_task._token) : _task._result;

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnCompleted(Action continuation) {
            if (_task._source is { } s)
                s.OnCompleted(AwaiterActions.InvokeContinuationDelegate, continuation,_task._token);
            else
                continuation();
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsafeOnCompleted(Action continuation) {
            if (_task._source is { } s)
                s.OnCompleted(AwaiterActions.InvokeContinuationDelegate, continuation,_task._token);
            else
                continuation();
        }

        /// <summary>
        /// If register manually continuation, you can use it instead of for compiler OnCompleted methods.
        /// </summary>
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SourceOnCompleted(Action<object?> continuation, object? state) {
            if (_task._source is { } s)
                s.OnCompleted(continuation, state, _task._token);
            else
                continuation(state);
        }
    }
}