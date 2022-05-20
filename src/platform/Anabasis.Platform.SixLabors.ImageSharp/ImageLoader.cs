using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Images.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Anabasis.Platform.SixLabors.ImageSharp;

public static class ImageLoader
{
    internal static bool Upload<TPixel, TFormat, TType>(this Image<TPixel> image,
        ISupportRawPixelUpload2D<TFormat, TType> texture, PixelFormatResolver resolver)
        where TPixel : unmanaged, IPixel<TPixel>
        where TFormat : Enum
        where TType : Enum {
        if (resolver.Resolve<TPixel, TFormat, TType>() is not var (format, type))
            return false;
        if (image.DangerousTryGetSinglePixelMemory(out Memory<TPixel> mem)) {
            texture.UploadPixels<TPixel>(0, ..image.Width, ..image.Height, format, type, mem.Span);
        } else {
            image.ProcessPixelRows(accessor => {
                for (int i = 0; i < accessor.Height; i++) {
                    texture.UploadPixels<TPixel>(0, ..accessor.Width, i..(i + 1), format, type, accessor.GetRowSpan(i));
                }
            });
        }

        return true;
    }

    public static void AddImageSharpUploader(this IServiceCollection services) {
        services.AddOptions<KnownPixelFormats>();
        services.TryAddSingleton<PixelFormatResolver>();
        services.TryAddScoped<IImageDataLoader, ImageSharpLoader>();
    }
}