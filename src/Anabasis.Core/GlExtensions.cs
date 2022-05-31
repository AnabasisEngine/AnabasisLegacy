using Silk.NET.OpenGL;

namespace Anabasis.Core;

public static class GlExtensions
{
    public static void ObjectLabel(this GL gl, ObjectIdentifier identifier, uint name, string label) =>
        gl.ObjectLabel(identifier, name, (uint)label.Length, label);

    public static string GetObjectLabel(this GL gl, ObjectIdentifier identifier, uint name) {
        gl.GetInteger(GetPName.MaxLabelLength, out int maxLength);
        gl.GetObjectLabel(identifier, name, (uint)Math.Min(maxLength, 128), out uint _, out string label);
        return label;
    }

    public static unsafe void DrawElements(this GL gl, PrimitiveType primitiveType, uint count,
        DrawElementsType indexType, nuint indexOffset) {
        gl.DrawElements(primitiveType, count, indexType, (void*)indexOffset);
    }

    public static unsafe void DrawElementsInstanced(this GL gl, PrimitiveType primitiveType, uint countPerInstance,
        DrawElementsType indexType, nuint indexOffset, uint instanceCount) {
        gl.DrawElementsInstanced(primitiveType, countPerInstance, indexType, (void*)indexOffset, instanceCount);
    }

    public static unsafe void DrawElementsBaseVertex(this GL gl, PrimitiveType primitiveType, uint count,
        DrawElementsType indexType, nuint indexOFfset, int baseVertex) {
        gl.DrawElementsBaseVertex(primitiveType, count, indexType, (void*)indexOFfset, baseVertex);
    }
}