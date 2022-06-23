using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics;

public static class GlExtensions
{
    public static void ObjectLabel(this GL gl, ObjectIdentifier identifier, uint name, string label) =>
        gl.ObjectLabel(identifier, name, (uint)label.Length, label);

    public static string GetObjectLabel(this GL gl, ObjectIdentifier identifier, uint name) {
        gl.GetInteger(GetPName.MaxLabelLength, out int maxLength);
        gl.GetObjectLabel(identifier, name, (uint)Math.Min(maxLength, 128), out uint _, out string label);
        return label;
    }

    public static unsafe void DrawElementsI(this GL gl, PrimitiveType primitiveType, uint count,
        DrawElementsType indexType, nuint indexOffset) {
        gl.DrawElements(primitiveType, count, indexType, (void*)indexOffset);
    }

    public static unsafe void DrawElementsInstancedI(this GL gl, PrimitiveType primitiveType, uint countPerInstance,
        DrawElementsType indexType, nuint indexOffset, uint instanceCount) {
        gl.DrawElementsInstanced(primitiveType, countPerInstance, indexType, (void*)indexOffset, instanceCount);
    }

    public static unsafe void DrawElementsBaseVertexI(this GL gl, PrimitiveType primitiveType, uint count,
        DrawElementsType indexType, nuint indexOffset, int baseVertex) {
        gl.DrawElementsBaseVertex(primitiveType, count, indexType, (void*)indexOffset, baseVertex);
    }

    public static void ThrowIfError(this GL gl, string function) {
        ErrorCode error = (ErrorCode)gl.GetError();
        if(error is ErrorCode.NoError)
            return;
        throw new GlException(error, function, "OpenGL Error");
    }

    public static uint CreateTexture(this GL gl, TextureTarget target) {
        gl.CreateTextures(target, 1, out uint handle);
        return handle;
    }
}