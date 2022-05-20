using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Images.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Anabasis.Platform.SixLabors.ImageSharp;

public static class ImageLoader
{
    private class TextureUploader<TPixel, TFormat, TType>
        where TPixel : unmanaged, IPixel<TPixel>
        where TFormat : Enum
        where TType : Enum
    {
        private readonly ISupportRawPixelUpload2D<TFormat, TType> _texture;
        private readonly int                                      _mipmap;
        private readonly TFormat                                  _format;
        private readonly TType                                    _type;

        public TextureUploader(ISupportRawPixelUpload2D<TFormat, TType> texture, int mipmap, TFormat format,
            TType type) {
            _texture = texture;
            _mipmap = mipmap;
            _format = format;
            _type = type;
        }

        public void Upload(Image<TPixel> image) {
            if (!image.DangerousTryGetSinglePixelMemory(out Memory<TPixel> memory)) {
                image.ProcessPixelRows(Run);
                return;
            }

            _texture.UploadPixels(_mipmap, ..image.Width, ..image.Height, _format, _type, memory.Span);
        }

        private void Run(PixelAccessor<TPixel> accessor) {
            for (int i = 0; i < accessor.Height; i++) {
                Span<TPixel> span = accessor.GetRowSpan(i);
                _texture.UploadPixels(_mipmap, ..accessor.Width, i..(i + 1), _format, _type, span);
            }
        }
    }

    internal static bool Upload<TPixel, TFormat, TType>(this Image<TPixel> image,
        ISupportRawPixelUpload2D<TFormat, TType> texture, PixelFormatResolver resolver)
        where TPixel : unmanaged, IPixel<TPixel>
        where TFormat : Enum
        where TType : Enum {
        if (resolver.Resolve<TPixel, TFormat, TType>() is not { } i) return false;
        new TextureUploader<TPixel, TFormat, TType>(texture, 0, i.UploadFormat, i.UploadType)
            .Upload(image);
        return true;
    }

    public static void AddImageSharpUploader(this IServiceCollection services) {
        services.TryAddSingleton<PixelFormatResolver>();
        services.TryAddScoped<IImageDataLoader, ImageSharpLoader>();
    }
}

public interface IPixelFormatInfo<TPixel, out TFormat, out TType>
    where TPixel : unmanaged, IPixel<TPixel>
    where TFormat : Enum
    where TType : Enum
{
    public TFormat UploadFormat { get; }
    public TType UploadType { get; }
}