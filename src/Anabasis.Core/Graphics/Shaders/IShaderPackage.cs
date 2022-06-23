using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Shaders;

/// <summary>
/// Marker interface for bindable objects which act as shader packages for rendering
/// This can be a complete shader program with both a vertex shader and a fragment shader,
/// or a pipeline of separable shader programs with at least one program bound to the vertex
/// and fragment stages.
/// </summary>
public interface IShaderPackage : IAnabasisBindableObject<GL>
{
    
}