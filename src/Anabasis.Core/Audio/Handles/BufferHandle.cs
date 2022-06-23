namespace Anabasis.Core.Audio.Handles;

public readonly record struct BufferHandle(uint Value) : IAnabasisHandle<AudioNatives>
{
    public void Free(AudioNatives api) {
        api.Al.DeleteBuffer(Value);
    }
}