using Anabasis.Graphics.Abstractions.Textures;
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

        public TextureUploader(ISupportRawPixelUpload2D<TFormat, TType> texture, int mipmap, TFormat format, TType type) {
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

            _texture.UploadPixels(_mipmap, Range.All, Range.All, _format, _type, memory.Span);
        }

        private void Run(PixelAccessor<TPixel> accessor) {
            for (int i = 0; i < accessor.Height; i++) {
                Span<TPixel> span = accessor.GetRowSpan(i);
                _texture.UploadPixels(_mipmap, Range.All, i..(i + 1), _format, _type, span);
            }
        }
    }

    public static void Upload<TPixel, TFormat, TType>(this Image<TPixel> image,
        ISupportRawPixelUpload2D<TFormat, TType> texture, PixelFormatResolver resolver)
        where TPixel : unmanaged, IPixel<TPixel>
        where TFormat : Enum
        where TType : Enum {
        IPixelFormatInfo<TPixel, TFormat, TType> resolver1 = resolver.Resolve<TPixel, TFormat, TType>();
        new TextureUploader<TPixel, TFormat, TType>(texture, 0, resolver1.UploadFormat, resolver1.UploadType)
            .Upload(image);
    }

    public static void AddImageSharpUploader(this IServiceCollection services) {
        services.TryAddSingleton<PixelFormatResolver>();
    }
}

public sealed class PixelFormatResolver
{
    private readonly IServiceProvider _serviceProvider;
    public PixelFormatResolver(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }

    public IPixelFormatInfo<TPixel, TFormat, TType> Resolve<TPixel, TFormat, TType>()
        where TPixel : unmanaged, IPixel<TPixel>
        where TFormat : Enum
        where TType : Enum =>
        _serviceProvider.GetRequiredService<IPixelFormatInfo<TPixel, TFormat, TType>>();
}

public interface IPixelFormatInfo<TPixel, out TFormat, out TType>
    where TPixel : unmanaged, IPixel<TPixel>
    where TFormat : Enum
    where TType : Enum
{
    public TFormat UploadFormat { get; }
    public TType UploadType { get; }
}