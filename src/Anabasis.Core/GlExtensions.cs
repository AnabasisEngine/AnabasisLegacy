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
}