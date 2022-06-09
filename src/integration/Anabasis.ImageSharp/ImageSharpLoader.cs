using System.Diagnostics.CodeAnalysis;
using Anabasis.Core.Textures;
using Anabasis.Tasks;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Anabasis.ImageSharp;

[SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters")]
public class ImageSharpLoader
{
    private static readonly Configuration ImageLoadConfiguration;
    private                 GL            _gl;

    static ImageSharpLoader() {
        ImageLoadConfiguration = Configuration.Default.Clone();
        ImageLoadConfiguration.PreferContiguousImageBuffers = true;
    }

    public ImageSharpLoader(GL gl) {
        _gl = gl;
    }

    public async AnabasisTask<Texture2D> LoadImageAsync(string file, uint mipmaps = 1,
        SizedInternalFormat format = SizedInternalFormat.Rgba8, CancellationToken cancellationToken = default) {
        await using FileStream str = File.OpenRead(file);
        return await LoadImageAsync(_gl, str, mipmaps, format, cancellationToken);
    }

    public static async AnabasisTask<Texture2D> LoadImageAsync(GL gl, Stream stream, uint mipmaps = 1,
        SizedInternalFormat format = SizedInternalFormat.Rgba8, CancellationToken cancellationToken = default) {
        using Image image = await Image.LoadAsync(ImageLoadConfiguration, stream, cancellationToken);
        await AnabasisTask.SwitchToMainThread(cancellationToken: cancellationToken);
        Texture2D texture = new(gl, (uint)image.Width, (uint)image.Height, mipmaps, format);
        UploadImage(image, texture);
        return texture;
    }

    public static void UploadImage(Image image, Texture2D texture, int mipmapLevel = 0) {
        switch (image) {
            case Image<Rgba32> i:
                UploadImage(i, texture, mipmapLevel);
                break;
            case Image<Bgra32> i:
                UploadImage(i, texture, mipmapLevel);
                break;
            default:
                UploadImage(image.CloneAs<Rgba32>(), texture, mipmapLevel);
                break;
        }
    }

    public static void UploadImage(Image<Rgba32> image, Texture2D texture, int mipmapLevel = 0) {
        DoUploadImage(image, texture, mipmapLevel, PixelFormat.Rgba, PixelType.UnsignedByte);
    }

    public static void UploadImage(Image<Bgra32> image, Texture2D texture, int mipmapLevel = 0) {
        DoUploadImage(image, texture, mipmapLevel, PixelFormat.Bgra, PixelType.UnsignedByte);
    }

    private static void DoUploadImage<T>(Image<T> image, Texture2D texture, int mipmapLevel, PixelFormat pixelFormat,
        PixelType pixelType)
        where T : unmanaged, IPixel<T> {
        if (image.DangerousTryGetSinglePixelMemory(out Memory<T> mem)) {
            texture.UploadPixels<T>(mipmapLevel, new Box2D<int>(0, 0, image.Width, image.Height),
                pixelFormat, pixelType, mem.Span);
            return;
        }

        image.ProcessPixelRows(accessor => {
            for (int i = 0; i < accessor.Height; i++) {
                texture.UploadPixels<T>(mipmapLevel, new Box2D<int>(0, i, accessor.Width, i + 1),
                    pixelFormat, pixelType, accessor.GetRowSpan(i));
            }
        });
    }
}