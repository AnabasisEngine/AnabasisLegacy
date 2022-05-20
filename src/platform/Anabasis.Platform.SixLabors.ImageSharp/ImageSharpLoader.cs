using System.Reflection;
using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Images.Abstractions;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Anabasis.Platform.SixLabors.ImageSharp;

public class ImageSharpLoader : IImageDataLoader
{
    internal static readonly Type                LoaderType = typeof(ImageLoader);
    internal static          MethodInfo?         UploadMethod;
    private readonly         PixelFormatResolver _resolver;
    private static readonly  Type                GenericSourceType = typeof(ImageSharpSource<>);
    private readonly         Configuration       _configuration;

    public ImageSharpLoader(IOptions<ImageSharpLoaderSettings> options, PixelFormatResolver resolver) {
        _resolver = resolver;
        _configuration = Configuration.Default.Clone();
        ImageSharpLoaderSettings settings = options.Value;
        _configuration.PreferContiguousImageBuffers = settings.PreferContiguousBuffers;
    }

    public async ValueTask<IImageDataSource> LoadAsync(Stream stream, CancellationToken cancellationToken = default) {
        Image img = await Image.LoadAsync(_configuration, stream, cancellationToken);
        return Create(img);
    }

    private IImageDataSource Create(Image img) {
        Type type = img.GetType();
        if (type.IsGenericType && typeof(Image<>) == type.GetGenericTypeDefinition()) {
            return Activator.CreateInstance(GenericSourceType.MakeGenericType(type.GetGenericArguments()),
                       BindingFlags.NonPublic, null, img, _resolver) as IImageDataSource ??
                   throw new InvalidOperationException();
        }

        return new ImageSharpSource<Rgba32>(img.CloneAs<Rgba32>(_configuration), _resolver);
    }

    public async ValueTask<ITexture2D> LoadAsync(ITextureSupport textureSupport, Stream stream,
        CancellationToken cancellationToken = default) {
        Image img = await Image.LoadAsync(_configuration, stream, cancellationToken);
        ITexture2D texture = await textureSupport.CreateTexture2DAsync(1, img.Width, img.Height);
        Create(img).UploadToTexture(texture);
        return texture;
    }
}