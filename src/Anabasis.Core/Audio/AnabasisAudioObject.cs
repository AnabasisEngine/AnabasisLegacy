namespace Anabasis.Core.Audio;

public class AnabasisAudioObject<THandle> : AnabasisNativeObject<AudioNatives, THandle>
    where THandle : struct, IAnabasisHandle<AudioNatives>
{
    public AnabasisAudioObject(AudioNatives api, THandle name) : base(api, name) { }
}