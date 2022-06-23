using Silk.NET.OpenAL;

namespace Anabasis.Core.Audio.Handles;

public readonly record struct DeviceHandle(uint Value) : IAnabasisHandle<AudioNatives>
{
    public unsafe void Free(AudioNatives api) {
        api.AlContext.CloseDevice((Device*)Value);
    }
}