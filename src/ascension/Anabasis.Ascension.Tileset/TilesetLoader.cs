using System.Collections.ObjectModel;
using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Images.Abstractions;
using Anabasis.Threading;

namespace Anabasis.Ascension.Tileset;

public sealed class TilesetLoader
{
    private readonly IImageDataLoader        _imageLoader;
    private readonly ITextureSupport         _support;
    private readonly ISupportCopyTextureData _copy;
    private readonly AnabasisTaskManager     _taskManager;

    public TilesetLoader(IImageDataLoader imageLoader, ITextureSupport support, ISupportCopyTextureData copy,
        AnabasisTaskManager taskManager) {
        _imageLoader = imageLoader;
        _support = support;
        _copy = copy;
        _taskManager = taskManager;
    }

    /// <summary>
    /// Loads a tileset atlas image, and processes it into a 2D texture array.
    /// </summary>
    /// <param name="stream">
    /// The source stream from which to load the atlas image.
    /// </param>
    /// <param name="tileWidth">
    /// Width of a single tile.
    /// </param>
    /// <param name="tileHeight">
    /// Height of a single tile.
    /// </param>
    /// <param name="levels">
    /// Number of mipmap levels to allocate for the final array.
    /// </param>
    /// <param name="spacing">
    /// Gap between tiles in the atlas.
    /// </param>
    /// <param name="padding">
    /// Border padding around the atlas image.
    /// </param>
    /// <param name="includeTileFunc">
    /// A filter for which tiles to include in the final texture array. If left <c>null</c>, all tiles will be included.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// The final texture array, and a lookup of final texture array layer from original linear tile coordinate.
    /// If <paramref name="includeTileFunc"/> is <c>null</c>, the returned dictionary will be null.
    /// </returns>
    public async ValueTask<(ITexture2DArray TextureArray, ReadOnlyDictionary<int, int>? TileIdLookup)>
        LoadTilesetAtlasAsync(
            Stream stream, int tileWidth, int tileHeight, int levels = 1, int spacing = 0, int padding = 0,
            Func<int, bool>? includeTileFunc = null, CancellationToken cancellationToken = default) {
        using ITexture2D atlas = await _imageLoader.LoadAsync(_support, stream, 1, cancellationToken);
        int noPaddingWidth = atlas.Width - 2 * padding;
        int noPaddingHeight = atlas.Height - 2 * padding;
        int tileRow = noPaddingWidth / tileWidth;
        int tileCol = noPaddingHeight / tileHeight;
        int tileCount = tileRow * tileCol;
        int finalCount = tileCount;
        Dictionary<int, int>? tileIdLookup = null;
        if (includeTileFunc != null) {
            finalCount = Enumerable.Range(0, tileCount)
                .Count(includeTileFunc);
            tileIdLookup = new Dictionary<int, int>(finalCount);
        } else {
            includeTileFunc = _ => true;
        }

        ITexture2DArray array = await _support.CreateTexture2DArrayAsync(levels, tileWidth, tileHeight, finalCount);
        for (int tileId = 0, layerId = 0; tileId < tileCount; tileId++) {
            if (!includeTileFunc(tileId))
                continue;
            cancellationToken.ThrowIfCancellationRequested();
            if (tileIdLookup != null) tileIdLookup[tileId] = layerId;
            int gridTileX = tileId % tileRow;
            int pixelTileX = padding + gridTileX * (tileWidth + spacing);

            int gridTileY = tileId / tileRow;
            int pixelTileY = padding + gridTileY * (tileWidth + spacing);
            _copy.CopyPixels(atlas, array, pixelTileX, pixelTileY, 0, 1, 0, 0, layerId, 1, (uint)tileWidth,
                (uint)tileHeight, 1);
            layerId++;
        }

        return (array, tileIdLookup != null ? new ReadOnlyDictionary<int, int>(tileIdLookup) : null);
    }

    /// <summary>
    /// Loads a tileset from a sequence of distinct tile images, which must all have the same dimension.
    /// </summary>
    /// <param name="streams">The source images to load.</param>
    /// <param name="tileWidth">The width of any one tile</param>
    /// <param name="tileHeight">The height of any one tile</param>
    /// <param name="levels">The number of mipmap levels to allocate</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The final texture array.</returns>
    public async ValueTask<ITexture2DArray> LoadTilesetTilesAsync(IEnumerable<Stream> streams, int tileWidth,
        int tileHeight, int levels = 1, CancellationToken cancellationToken = default) {
        if (!streams.TryGetNonEnumeratedCount(out int count)) {
            Stream[] streamsArray = streams.ToArray();
            count = streamsArray.Length;
            streams = streamsArray;
        }

        return await LoadTilesetTilesAsync(streams, tileWidth, tileHeight, levels, count, cancellationToken);
    }

    /// <summary>
    /// Loads a tileset from a sequence of distinct tile images, which must all have the same dimension.
    /// </summary>
    /// <param name="streams">The source images to load.</param>
    /// <param name="tileWidth">The width of any one tile</param>
    /// <param name="tileHeight">The height of any one tile</param>
    /// <param name="levels">The number of mipmap levels to allocate</param>
    /// <param name="count">The number of tiles to load</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The final texture array.</returns>
    public async Task<ITexture2DArray> LoadTilesetTilesAsync(IEnumerable<Stream> streams, int tileWidth, int tileHeight,
        int levels, int count, CancellationToken cancellationToken = default) {
        ITexture2DArray array = await _support.CreateTexture2DArrayAsync(levels, tileWidth, tileHeight, count);
        int layer = 0;
        foreach (Stream stream in streams) {
            cancellationToken.ThrowIfCancellationRequested();
            IImageDataSource source = await _imageLoader.LoadAsync(stream, cancellationToken);
            await _taskManager.Yield();
            ITextureView2D view = array.Upload(layer++);
            source.UploadToTexture(view);
            if (layer >= count)
                break;
        }

        return array;
    }

    /// <summary>
    /// Loads a tileset from a sequence of distinct tile images, which must all have the same dimension.
    /// </summary>
    /// <param name="streams">The source images to load.</param>
    /// <param name="tileWidth">The width of any one tile</param>
    /// <param name="tileHeight">The height of any one tile</param>
    /// <param name="levels">The number of mipmap levels to allocate</param>
    /// <param name="count">The number of tiles to load</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The final texture array.</returns>
    public async Task<ITexture2DArray> LoadTilesetTilesAsync(IAsyncEnumerable<Stream> streams, int tileWidth,
        int tileHeight, int levels, int count, CancellationToken cancellationToken = default) {
        ITexture2DArray array = await _support.CreateTexture2DArrayAsync(levels, tileWidth, tileHeight, count);
        int layer = 0;
        await foreach (Stream stream in streams.WithCancellation(cancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();
            IImageDataSource source = await _imageLoader.LoadAsync(stream, cancellationToken);
            await _taskManager.Yield();
            ITextureView2D view = array.Upload(layer++);
            source.UploadToTexture(view);
            if (layer >= count)
                break;
        }

        return array;
    }
}