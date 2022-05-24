using System.Reflection;
using System.Runtime.InteropServices;
using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Images.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = Anabasis.Graphics.Abstractions.Color;

namespace Anabasis.Platform.SixLabors.ImageSharp;

internal sealed class ImageSharpSource<TPixel> : IImageDataSource
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly Image<TPixel>       _img;
    private readonly PixelFormatResolver _resolver;

    public ImageSharpSource(Image<TPixel> img, PixelFormatResolver resolver) {
        _img = img;
        _resolver = resolver;
    }

    public void UploadToTexture(ITextureView2D texture) {
        Type type = texture.GetType();
        if (type.GetInterfaces().SingleOrDefault(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISupportRawPixelUpload2D<,>)) is { } iface) {
            ImageSharpLoader.UploadMethod ??=
                ImageSharpLoader.LoaderType.GetMethod(nameof(ImageLoader.Upload),
                    BindingFlags.Static | BindingFlags.NonPublic) ??
                throw new InvalidOperationException();
            if (!ImageSharpLoader.UploadMethod.IsGenericMethodDefinition) {
                throw new InvalidOperationException();
            }

            if ((bool)(ImageSharpLoader.UploadMethod
                    .MakeGenericMethod(typeof(TPixel), iface.GetGenericArguments()[0], iface.GetGenericArguments()[1])
                    .Invoke(null, new object[] { _img, texture, _resolver, }) ?? false))
                return;
        }

        // Rgba32 has the same internal representation as our Color so we can do some unsafe shit here
        if (typeof(TPixel) == typeof(Rgba32)) {
            // If we are already Rgba32 then cast
            Image<Rgba32> rgba = (Image<Rgba32>)(Image)_img;
            UploadRgba(texture, rgba);
        } else {
            // Otherwise, clone and ensure clone gets disposed. THIS IS SLOW
            using Image<Rgba32> rgba = _img.CloneAs<Rgba32>();
            UploadRgba(texture, rgba);
        }
    }

    private static void UploadRgba(ITextureView2D texture, Image<Rgba32> image) {
        if (image.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> memory)) {
            texture.UploadPixels(0, 0, (uint)image.Width, 0, (uint)image.Height,
                MemoryMarshal.Cast<Rgba32, Color>(memory.Span));
        } else {
            image.ProcessPixelRows(accessor => {
                for (int i = 0; i < accessor.Height; i++) {
                    Span<Rgba32> span = accessor.GetRowSpan(i);
                    texture.UploadPixels(0, 0, (uint)accessor.Width, i, 1, MemoryMarshal.Cast<Rgba32, Color>(span));
                }
            });
        }
    }
}