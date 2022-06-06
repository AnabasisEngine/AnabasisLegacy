using Anabasis.Core.Shaders;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Rendering;

/// <summary>
/// A render "batch" - comprises a bound <see cref="IShaderPackage"/> and bound <see cref="VertexArray"/>
/// Includes basic rendering functions such as <see cref="DrawArrays"/> and <see cref="DrawElements(Silk.NET.OpenGL.PrimitiveType,uint,uint)"/>
/// </summary>
public sealed class RenderBatch : IDisposable
{
    private readonly GL          _gl;
    private readonly IDisposable _shaderBinding;
    private readonly VertexArray _vertexArray;
    private readonly IDisposable _vaoBinding;

    private RenderBatch(GL gl, IDisposable shaderBinding, VertexArray vertexArray, IDisposable vaoBinding) {
        _gl = gl;
        _shaderBinding = shaderBinding;
        _vertexArray = vertexArray;
        _vaoBinding = vaoBinding;
    }

    /// <summary>
    /// Open a new rendering batch, binding the given <paramref name="shaderPackage"/> and <paramref name="array"/>
    /// </summary>
    /// <param name="gl">The rendering API</param>
    /// <param name="shaderPackage">The shader package to bind</param>
    /// <param name="array">The vertex array to bind</param>
    /// <returns>An instance of <see cref="RenderBatch"/></returns>
    public static RenderBatch Begin(GL gl, IShaderPackage shaderPackage, VertexArray array) =>
        new(gl, shaderPackage.Use(), array, array.Use());

    public void DrawArrays(PrimitiveType primitiveType, int first, uint count) =>
        _gl.DrawArrays(primitiveType, first, count);

    public unsafe void DrawElements(PrimitiveType primitiveType, uint count, DrawElementsType indexType,
        uint indexOffset) =>
        _gl.DrawElements(primitiveType, count, indexType, (void*)indexOffset);

    public void DrawElements(PrimitiveType primitiveType, uint count, uint indexOffset) {
        DrawElementsType elementsType = _vertexArray.IndexType ?? throw new InvalidOperationException();
        DrawElements(primitiveType, count, elementsType, (uint)(indexOffset * elementsType switch {
            DrawElementsType.UnsignedByte => 1,
            DrawElementsType.UnsignedShort => 2,
            DrawElementsType.UnsignedInt => 4,
            _ => throw new ArgumentOutOfRangeException(),
        }));
    }

    public void DrawElements(PrimitiveType primitiveType, uint count, ReadOnlySpan<ushort> indices) {
        _gl.DrawElements(primitiveType, count, DrawElementsType.UnsignedShort, indices);
    }

    public void DrawElements(PrimitiveType primitiveType, uint count, ReadOnlySpan<byte> indices) {
        _gl.DrawElements(primitiveType, count, DrawElementsType.UnsignedByte, indices);
    }

    public void DrawElements(PrimitiveType primitiveType, uint count, ReadOnlySpan<uint> indices) {
        _gl.DrawElements(primitiveType, count, DrawElementsType.UnsignedInt, indices);
    }

    public void Dispose() {
        _shaderBinding.Dispose();
        _vaoBinding.Dispose();
    }
}