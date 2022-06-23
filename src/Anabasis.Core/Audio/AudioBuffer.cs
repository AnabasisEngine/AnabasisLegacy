using Anabasis.Core.Audio.Handles;
using Silk.NET.OpenAL;

namespace Anabasis.Core.Audio;

public sealed class AudioBuffer : AnabasisNativeObject<AudioNatives, BufferHandle>
{
    internal static readonly Dictionary<uint, AudioBuffer> KnownBuffers = new();

    public AudioBuffer(AudioNatives api) : base(api, new BufferHandle(api.Al.GenBuffer())) {
        api.Al.ThrowIfError(nameof(AL.GenBuffer));
        KnownBuffers[Handle.Value] = this;
    }

    public unsafe void LoadData<T>(ReadOnlySpan<T> data, bool mono, int frequency)
        where T : unmanaged {
        BufferFormat format = (mono, sizeof(T)) switch {
            (true, sizeof(byte)) => BufferFormat.Mono8,
            (false, sizeof(byte)) => BufferFormat.Stereo8,
            (true, sizeof(short)) => BufferFormat.Mono16,
            (false, sizeof(short)) => BufferFormat.Stereo16,
            _ => throw new InvalidOperationException("Unknown buffer format"),
        };
        LoadData(data, format, frequency);
    }

    public unsafe void LoadData<T>(ReadOnlySpan<T> data, BufferFormat format, int frequency)
        where T : unmanaged {
        fixed (void* d = data) {
            Api.Al.BufferData(Handle.Value, format, d, data.Length * sizeof(T), frequency);
        }
    }

    protected override void Dispose(bool disposing) {
        if (disposing)
            KnownBuffers.Remove(Handle.Value);
        base.Dispose(disposing);
    }
}