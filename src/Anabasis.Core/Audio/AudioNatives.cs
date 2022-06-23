using Anabasis.Core.Audio.Handles;
using Silk.NET.OpenAL;

namespace Anabasis.Core.Audio;

public sealed class AudioNatives : IDisposable
{
    public AudioNatives(bool soft = false) {
        Al = AL.GetApi(soft);
        AlContext = ALContext.GetApi(soft);
    }

    public readonly AL        Al;
    public readonly ALContext AlContext;

    public unsafe AudioDevice OpenDevice(string? name) {
        uint device = (uint)AlContext.OpenDevice(name);
        if (device == 0)
            throw new Exception();
        return new AudioDevice(this, new DeviceHandle(device));
    }

    public void Dispose() {
        Al.Dispose();
        AlContext.Dispose();
    }
}

public static class AudioNativesExtensions
{
    public static void ThrowIfError(this AL al, string function) {
        AudioError error = al.GetError();
        if (error != AudioError.NoError)
            throw new AlException((AlError)error, function, "OpenAL Error");
    }
    public static unsafe void ThrowIfError(this AudioDevice device, string function) {
        ContextError error = device.Api.AlContext.GetError((Device*)device.Handle.Value);
        if (error != ContextError.NoError)
            throw new AlException((AlError)(error + 100000), function, "OpenAL Error");
    }
}