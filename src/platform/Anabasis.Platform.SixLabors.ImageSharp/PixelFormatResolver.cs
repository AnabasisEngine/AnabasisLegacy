using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.PixelFormats;

namespace Anabasis.Platform.SixLabors.ImageSharp;

public sealed class PixelFormatResolver
{
    private readonly IServiceProvider _serviceProvider;

    public PixelFormatResolver(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }

    public (TFormat UploadFormat, TType UploadType)? Resolve<TPixel, TFormat, TType>()
        where TPixel : unmanaged, IPixel<TPixel>
        where TFormat : Enum
        where TType : Enum => _serviceProvider.GetRequiredService<IOptions<KnownPixelFormats<TFormat, TType>>>().Value
        .KnownFormats[typeof(TPixel)];
}

public class KnownPixelFormats<TFormat, TType>
{
    public Dictionary<Type, (TFormat, TType)> KnownFormats { get; } = new();

    public void Register<TPixel>(TFormat format, TType type) => KnownFormats[typeof(TPixel)] = (format, type);
}