namespace Anabasis.Core.Audio.Handles;

public readonly record struct SourceHandle(uint Value) : IAnabasisHandle<AudioNatives>
{
    public void Free(AudioNatives api) {
        api.Al.DeleteSource(Value);
    }
}