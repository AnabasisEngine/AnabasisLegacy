using System.Diagnostics.CodeAnalysis;
using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Platform.Abstractions.Resources;

namespace Anabasis.Graphics.Abstractions;

public sealed class DrawPipeline : IDisposable
{
    internal DrawPipeline(ShaderProgram shaderProgram, IGraphicsDevice graphicsDevice) {
        VertexArray = graphicsDevice.CreateVertexArray();
        ShaderProgram = shaderProgram;
        GraphicsDevice = graphicsDevice;
    }

    public IVertexArray VertexArray { get; }
    public ShaderProgram ShaderProgram { get; }

    private LinkedList<IPlatformResource> VertexBuffers { get; } = new();
    private IPlatformResource? IndexBuffer { get; set; }

    public IGraphicsDevice GraphicsDevice { get; }

    [MemberNotNull(nameof(IndexBuffer))]
    public IBufferObject<T> CreateIndexBuffer<T>(int count, ReadOnlySpan<T> span = default,
        BufferAccess flags = BufferAccess.None)
        where T : unmanaged {
        IndexBuffer?.Dispose();
        IBufferObject<T> idxBuf = GraphicsDevice.CreateBuffer<T>(BufferType.IndexBuffer);
        IndexBuffer = idxBuf;
        idxBuf.Allocate(count, span, flags);
        VertexArray.BindIndexBuffer(idxBuf);
        return idxBuf;
    }

    public IBufferObject<T> CreateIndexBuffer<T>(int count, StatelessSpanAction<T> action,
        BufferAccess flags = BufferAccess.DefaultMap,
        IVertexBufferFormatter<T>? formatter = null)
        where T : unmanaged {
        IBufferObject<T> idxBuf = GraphicsDevice.CreateBuffer<T>(BufferType.VertexBuffer);
        idxBuf.LoadData(0, count, action, flags);
        VertexArray.BindIndexBuffer(idxBuf);
        IndexBuffer = idxBuf;
        return idxBuf;
    }

    public IBufferObject<T> CreateVertexBuffer<T>(int count, ReadOnlySpan<T> span = default,
        BufferAccess flags = BufferAccess.None,
        IVertexBufferFormatter<T>? formatter = null)
        where T : unmanaged {
        IBufferObject<T> buf = GraphicsDevice.CreateBuffer<T>(BufferType.VertexBuffer);
        buf.Allocate(count, span, flags);
        ShaderProgram.FormatBuffer(VertexArray, buf, formatter);
        VertexBuffers.AddLast(buf);
        return buf;
    }

    public IBufferObject<T> CreateVertexBuffer<T>(int count, StatelessSpanAction<T> action,
        BufferAccess flags = BufferAccess.DefaultMap,
        IVertexBufferFormatter<T>? formatter = null)
        where T : unmanaged {
        IBufferObject<T> buf = GraphicsDevice.CreateBuffer<T>(BufferType.VertexBuffer);
        buf.LoadData(0, count, action, flags);
        ShaderProgram.FormatBuffer(VertexArray, buf, formatter);
        VertexBuffers.AddLast(buf);
        return buf;
    }

    public void Dispose() {
        VertexArray?.Dispose();
        IndexBuffer?.Dispose();
        foreach (IPlatformResource buffer in VertexBuffers) {
            buffer.Dispose();
        }
    }

    public void DrawArrays(DrawMode drawMode, int first, uint count) {
        using (ShaderProgram.Use())
        using (VertexArray.Use())
            VertexArray.DrawArrays(drawMode, first, count);
    }

    public void DrawElements(DrawMode drawMode, uint count, uint indexOffset) {
        using (ShaderProgram.Use())
        using (VertexArray.Use())
            VertexArray.DrawElements(drawMode, count, indexOffset);
    }

    public void DrawArraysInstanced(DrawMode drawMode, int first, uint count, uint instances) {
        using (ShaderProgram.Use())
        using (VertexArray.Use())
            VertexArray.DrawArraysInstanced(drawMode, first, count, instances);
    }

    public void DrawElementsInstanced(DrawMode drawMode, uint count, uint indexOffset, uint instanceCount) {
        using (ShaderProgram.Use())
        using (VertexArray.Use())
            VertexArray.DrawElementsInstanced(drawMode, count, indexOffset, instanceCount);
    }


    public void DrawElementsBaseVertex(DrawMode drawMode, uint count, uint indexOffset, int baseVertex) {
        using (ShaderProgram.Use())
        using (VertexArray.Use())
            VertexArray.DrawElementsBaseVertex(drawMode, count, indexOffset, baseVertex);
    }

    public void DrawElementsInstancedBaseVertex(DrawMode drawMode, uint count, uint indexOffset, uint instanceCount,
        int baseVertex) {
        using (ShaderProgram.Use())
        using (VertexArray.Use())
            VertexArray.DrawElementsInstancedBaseVertex(drawMode, count, indexOffset, instanceCount, baseVertex);
    }

    public void DrawElementsInstancedBaseVertexBaseInstance(DrawMode drawMode, uint count, uint indexOffset,
        uint instanceCount, int baseVertex, uint baseInstance) {
        using (ShaderProgram.Use())
        using (VertexArray.Use())
            VertexArray.DrawElementsInstancedBaseVertexBaseInstance(drawMode, count, indexOffset, instanceCount,
                baseVertex, baseInstance);
    }
}