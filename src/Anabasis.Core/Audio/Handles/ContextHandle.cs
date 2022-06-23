using Silk.NET.OpenAL;

namespace Anabasis.Core.Audio.Handles;

public readonly record struct ContextHandle(uint Value) : IAnabasisBindableHandle<AudioNatives>
{
    public unsafe void Free(AudioNatives api) {
        api.AlContext.DestroyContext((Context*)Value);
    }

    public unsafe bool Use(AudioNatives api) => api.AlContext.MakeContextCurrent((Context*)Value);

    void IBindable<AudioNatives>.Use(AudioNatives api) {
        Use(api);
    }
}