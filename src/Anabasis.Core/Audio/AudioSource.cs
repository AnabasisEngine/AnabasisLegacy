using Anabasis.Core.Audio.Handles;
using Anabasis.Tasks;
using Silk.NET.OpenAL;

namespace Anabasis.Core.Audio;

public sealed class AudioSource : AnabasisNativeObject<AudioNatives, SourceHandle>
{
    public AudioSource(AudioNatives api) : base(api, new SourceHandle(api.Al.GenSource())) {
        api.Al.ThrowIfError(nameof(AL.GenSource));
    }

    private bool _looping;

    public bool Looping {
        get => _looping;
        set {
            Api.Al.SetSourceProperty(Handle.Value, SourceBoolean.Looping, _looping = value);
            Api.Al.ThrowIfError(nameof(AL.SetSourceProperty));
        }
    }

    public int BuffersProcessed {
        get {
            Api.Al.GetSourceProperty(Handle.Value, GetSourceInteger.BuffersProcessed, out int value);
            Api.Al.ThrowIfError(nameof(AL.GetSourceProperty));
            return value;
        }
    }

    public SourceState State {
        get {
            Api.Al.GetSourceProperty(Handle.Value, GetSourceInteger.SourceState, out int state);
            Api.Al.ThrowIfError(nameof(AL.GetSourceProperty));
            return (SourceState)state;
        }
    }

    public void SetBuffer(AudioBuffer buffer) {
        Api.Al.SetSourceProperty(Handle.Value, SourceInteger.Buffer, buffer.Handle.Value);
        Api.Al.ThrowIfError(nameof(AL.SetSourceProperty));
    }

    public void Play() {
        Api.Al.SourcePlay(Handle.Value);
        Api.Al.ThrowIfError(nameof(AL.SourcePlay));
    }

    public void Pause() {
        Api.Al.SourcePause(Handle.Value);
        Api.Al.ThrowIfError(nameof(AL.SourcePause));
    }

    public void Rewind() {
        Api.Al.SourceRewind(Handle.Value);
        Api.Al.ThrowIfError(nameof(AL.SourceRewind));
    }

    public void Stop() {
        Api.Al.SourceStop(Handle.Value);
        Api.Al.ThrowIfError(nameof(AL.SourceStop));
    }

    public AnabasisTask PlayOnceAsync(CancellationToken cancellationToken) {
        Play();
        return WaitForEndAsync(cancellationToken);
    }

    public async AnabasisTask PlayAsync(CancellationToken stopToken) {
        await using (stopToken.RegisterOnGameLoop(a => ((AudioSource)a!).Stop(), this)) {
            Play();
            await WaitForEndAsync(CancellationToken.None);
        }
    }

    public async AnabasisTask WaitForEndAsync(CancellationToken cancellationToken) {
        while (State != SourceState.Stopped) {
            cancellationToken.ThrowIfCancellationRequested();
            await AnabasisTask.Yield(AnabasisPlatformLoopStep.Update, cancellationToken);
        }
    }

    public unsafe void QueueBuffers(params AudioBuffer[] buffer) {
        uint* ids = stackalloc uint[buffer.Length];
        for (int i = 0; i < buffer.Length; i++) {
            ids[i] = buffer[i].Handle.Value;
        }

        Api.Al.SourceQueueBuffers(Handle.Value, buffer.Length, ids);
        Api.Al.ThrowIfError(nameof(AL.SourceQueueBuffers));
    }

    public unsafe void QueueBuffer(AudioBuffer buffer) {
        uint* id = stackalloc uint[] { buffer.Handle.Value };
        Api.Al.SourceQueueBuffers(Handle.Value, 1, id);
        Api.Al.ThrowIfError(nameof(AL.SourceQueueBuffers));
    }

    public unsafe void PopBuffers(Span<AudioBuffer> buffers) {
        uint* ids = stackalloc uint[buffers.Length];
        Api.Al.SourceUnqueueBuffers(Handle.Value, buffers.Length, ids);
        Api.Al.ThrowIfError(nameof(AL.SourceUnqueueBuffers));
        for (int i = 0; i < buffers.Length; i++) {
            if (!AudioBuffer.KnownBuffers.TryGetValue(ids[i], out AudioBuffer? buf))
                throw new Exception();
            buffers[i] = buf;
        }
    }

    public unsafe void PopBuffer(out AudioBuffer buffer) {
        uint id = 0;
        Api.Al.SourceUnqueueBuffers(Handle.Value, 1, &id);
        Api.Al.ThrowIfError(nameof(AL.SourceUnqueueBuffers));
        if (!AudioBuffer.KnownBuffers.TryGetValue(id, out AudioBuffer? buf))
            throw new Exception();
        buffer = buf;
    }
}