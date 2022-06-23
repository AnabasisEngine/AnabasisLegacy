using Anabasis.Core.Audio.Handles;

namespace Anabasis.Core.Audio;

public sealed class AudioContext : AnabasisNativeObject<AudioNatives, ContextHandle>
{
    public AudioContext(AudioNatives api, ContextHandle name) : base(api, name) { }

    public IDisposable Use(out bool success) {
        success = Handle.Use(Api);
        return new GenericDisposer(() => { });
    }
}