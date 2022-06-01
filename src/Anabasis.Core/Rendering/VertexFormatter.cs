using Anabasis.Core.Handles;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Rendering;

public delegate void VertexFormatter(VertexArrayBindingIndex bindingIndex, GL gl, VertexArrayHandle vertexArray);