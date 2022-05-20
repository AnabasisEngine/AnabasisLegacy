using Anabasis.Platform.SixLabors.ImageSharp;
using Microsoft.Extensions.DependencyInjection;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp.PixelFormats;

namespace Anabasis.Platform.Silk.SixLabors.ImageSharp;

public static class SilkImageSharpFormatProvider
{
    private class FormatResolver<TPixel> : IPixelFormatInfo<TPixel, PixelFormat, PixelType>
        where TPixel : unmanaged, IPixel<TPixel>
    {
        public FormatResolver(PixelFormat uploadFormat, PixelType uploadType) {
            UploadFormat = uploadFormat;
            UploadType = uploadType;
        }
        public PixelFormat UploadFormat { get; }
        public PixelType UploadType { get; }
    }

    private static void AddKnownFormat<TPixel>(this IServiceCollection services, PixelFormat format, PixelType type)
        where TPixel : unmanaged, IPixel<TPixel> {
        services.AddSingleton<IPixelFormatInfo<TPixel, PixelFormat, PixelType>>(_ =>
            new FormatResolver<TPixel>(format, type));
    }
    
    public static void AddSilkImageSharpSupport(this IServiceCollection services) {
        services.AddImageSharpUploader();
        services.AddKnownFormat<Rgba32>(PixelFormat.Rgba, PixelType.UnsignedByte);
    }
}