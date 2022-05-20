using Anabasis.Platform.SixLabors.ImageSharp;
using Microsoft.Extensions.DependencyInjection;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp.PixelFormats;

namespace Anabasis.Platform.Silk.SixLabors.ImageSharp;

public static class SilkImageSharpFormatProvider
{
    public static void AddSilkImageSharpSupport(this IServiceCollection services) {
        services.Configure<KnownPixelFormats>(k => {
            k.Register<Rgba32>(PixelFormat.Rgba, PixelType.UnsignedByte);
        });
    }
}