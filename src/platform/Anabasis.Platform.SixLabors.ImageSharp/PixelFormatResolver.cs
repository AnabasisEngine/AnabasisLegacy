using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.PixelFormats;

namespace Anabasis.Platform.SixLabors.ImageSharp;

public sealed class PixelFormatResolver
{
    private readonly KnownPixelFormats _formats;

    public PixelFormatResolver(IOptions<KnownPixelFormats> options) {
        _formats = options.Value;
    }

    public (TFormat UploadFormat, TType UploadType)? Resolve<TPixel, TFormat, TType>()
        where TPixel : unmanaged, IPixel<TPixel>
        where TFormat : Enum
        where TType : Enum =>
        _formats.KnownFormats.TryGetValue(typeof(TPixel), out (Enum format, Enum type) tuple)
        && tuple.format is TFormat f && tuple.type is TType t
            ? (f, t)
            : null;
}

public class KnownPixelFormats
{
    public Dictionary<Type, (Enum format, Enum type)> KnownFormats { get; } = new();

    public void Register<TPixel>(Enum format, Enum type) => KnownFormats[typeof(TPixel)] = (format, type);
}