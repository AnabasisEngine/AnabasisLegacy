using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Platform.Abstractions;
using Microsoft.CodeAnalysis.PooledObjects;
using Silk.NET.Maths;

namespace Anabasis.Ascension.SpriteBatch;

public ref struct SpriteBatch
{
    private readonly int                       _baseOffset;
    private readonly SpriteBatcher             _batcher;
    private readonly Span<VertexSpecification> _currentWorkingBuffer;
    private readonly uint                      _indexOffset;
    private          uint                      _currentIndex;
    private          int                       _currentVertex;
    private readonly Span<ushort>              _indexWorkingBuffer;
    private readonly LinkedList<BatchItem>     _items = new();

    internal SpriteBatch(Span<VertexSpecification> fullBuffer, int offset, int length, SpriteBatcher batcher,
        Span<ushort> indexWorkingBuffer) {
        _currentWorkingBuffer = fullBuffer.Slice(_baseOffset, length);
        _baseOffset = offset;
        _batcher = batcher;
        _indexOffset = (uint)(_baseOffset * 3 / 2);
        _indexWorkingBuffer = indexWorkingBuffer.Slice((int)_indexOffset, length * 3 / 2);
    }

    internal class BatchItem
    {
        private static readonly ObjectPool<BatchItem> Pool = new(() => new BatchItem());

        public static BatchItem Allocate(in Span<VertexSpecification> buffer, int offset, in Rectangle<float> bounds,
            in Color color, in Rectangle<ushort> texCoordRect, float depth, ITexture2D texture,
            Span<ushort> indexLocation) {
            BatchItem item = Pool.Allocate();
            item.Set(buffer, offset, bounds, color, texCoordRect, depth, texture, indexLocation);
            return item;
        }

        public void Free() => Pool.Free(this);

        public ITexture2D Texture = null!;

        private void Set(in Span<VertexSpecification> buffer, int offset, in Rectangle<float> bounds, in Color color,
            in Rectangle<ushort> texCoordRect, float depth, ITexture2D texture,
            Span<ushort> indexLocation) {
            Texture = texture;
            Span<VertexSpecification> verticesMemory = buffer.Slice(offset, 4);
            verticesMemory[0].Position = new Vector3D<float>(bounds.Origin, depth);
            verticesMemory[0].Color = color;
            verticesMemory[0].TexCoord = Texture.NormalizeTexCoordUshort(texCoordRect.Origin);

            verticesMemory[1].Position =
                new Vector3D<float>(bounds.Origin + new Vector2D<float>(bounds.Size.X, 0), depth);
            verticesMemory[1].Color = color;
            verticesMemory[0].TexCoord =
                Texture.NormalizeTexCoordUshort(texCoordRect.Origin + new Vector2D<ushort>(texCoordRect.Size.X, 0));

            verticesMemory[2].Position =
                new Vector3D<float>(bounds.Origin + new Vector2D<float>(0, bounds.Size.Y), depth);
            verticesMemory[2].Color = color;
            verticesMemory[0].TexCoord =
                Texture.NormalizeTexCoordUshort(texCoordRect.Origin + new Vector2D<ushort>(0, texCoordRect.Size.Y));

            verticesMemory[3].Position = new Vector3D<float>(bounds.Max, depth);
            verticesMemory[3].Color = color;
            verticesMemory[0].TexCoord = Texture.NormalizeTexCoordUshort(texCoordRect.Max);

            // indices are 0,1,3,1,2,3, adjusted for offset within working buffer
            indexLocation[0] = (ushort)(0 + offset);
            indexLocation[1] = (ushort)(1 + offset);
            indexLocation[2] = (ushort)(3 + offset);
            indexLocation[3] = (ushort)(1 + offset);
            indexLocation[4] = (ushort)(2 + offset);
            indexLocation[5] = (ushort)(3 + offset);
        }
    }

    public void Draw(ITexture2D texture, float x, float y, float w, float h, in Color color, ushort srcX, ushort srcY,
        ushort srcW, ushort srcH, float depth) {
        _items.AddLast(BatchItem.Allocate(_currentWorkingBuffer, _currentVertex, new Rectangle<float>(x, y, w, h),
            color,
            new Rectangle<ushort>(srcX, srcY, srcW, srcH), depth, texture,
            _indexWorkingBuffer.Slice((int)_currentIndex, 6)));
        _currentVertex += 4;
        _currentIndex += 6;
    }

    public void Dispose() {
        IPlatformHandle? lastTexture = null;
        uint count = 0, lastBlock = 0;
        foreach (BatchItem item in _items) {
            try {
                // six more vertices
                count += 6;
                // check if we need to flush and change texture
                if (item.Texture.Handle == lastTexture)
                    continue;

                // render all vertices for this texture
                _batcher.Pipeline.DrawElementsBaseVertex(DrawMode.Triangles, count, _indexOffset + lastBlock,
                    _baseOffset);

                // update texture binding
                lastTexture = item.Texture.Handle;
                _batcher.Shader.Texture.BindTexture(item.Texture);
                lastBlock += count;
                count = 0;
            }
            finally {
                item.Free();
            }
        }

        _batcher.Pipeline.DrawElementsBaseVertex(DrawMode.Triangles, count, _indexOffset + lastBlock, _baseOffset);
        _items.Clear();
    }
}