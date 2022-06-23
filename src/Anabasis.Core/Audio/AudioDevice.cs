using Anabasis.Core.Audio.Handles;
using Silk.NET.OpenAL;

namespace Anabasis.Core.Audio;

public sealed class AudioDevice : AnabasisNativeObject<AudioNatives, DeviceHandle>
{
    public AudioDevice(AudioNatives api, DeviceHandle name) : base(api, name) { }

    public unsafe AudioContext CreateContext() =>
        new(Api, new ContextHandle((uint)Api.AlContext.CreateContext((Device*)Handle.Value, null)));
}