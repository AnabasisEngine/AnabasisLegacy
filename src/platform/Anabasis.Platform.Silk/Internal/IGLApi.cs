using System.Numerics;
using Anabasis.Platform.Abstractions;
using Anabasis.Platform.Silk.Buffers;
using Anabasis.Platform.Silk.Shader;
using Anabasis.Platform.Silk.Textures;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

/// <summary>
/// A wrapper over <see cref="GL"/> with the aim of using value objects to remove primitive obsession issues.
/// Set up as an interface to allow mocking for unit testing, in practice should always be an instance of <see cref="GlApi"/>
/// </summary>
public interface IGlApi : IDisposable
{
    int GetAttribLocation(ProgramHandle program, string name);

    void BufferData<T0>(BufferTargetARB target, ReadOnlySpan<T0> data, BufferUsageARB usage)
        where T0 : unmanaged;

    void BufferSubData<T0>(BufferTargetARB target, nint offset, ReadOnlySpan<T0> data)
        where T0 : unmanaged;

    void NamedBufferData<T>(BufferObjectHandle buffer, ReadOnlySpan<T> data, BufferUsageARB usage)
        where T : unmanaged;

    void NamedBufferSubData<T>(BufferObjectHandle buffer, nint offset, ReadOnlySpan<T> data)
        where T : unmanaged;

    void EnableVertexArrayAttrib(VertexArrayHandle handle, uint layout);

    void VertexArrayAttribFormat(VertexArrayHandle handle, uint layout, int count, VertexAttribType pointerType, bool b,
        uint offset);

    void VertexArrayAttribBinding(VertexArrayHandle handle, uint layout, SilkBindingIndex bindingIndex);
    BufferObjectHandle CreateBuffer();
    void DeleteBuffer(BufferObjectHandle handle);
    void BindBuffer(BufferTargetARB target, BufferObjectHandle handle);
    VertexArrayHandle CreateVertexArray();
    void BindVertexArray(VertexArrayHandle handle);
    void DeleteVertexArray(VertexArrayHandle handle);

    void VertexArrayVertexBuffer(VertexArrayHandle handle, SilkBindingIndex idx, BufferObjectHandle buffer, int i,
        uint sizeOf);

    void VertexArrayElementBuffer(VertexArrayHandle handle, BufferObjectHandle buffer);
    void DrawArrays(PrimitiveType primitiveType, int first, uint count);
    void DrawElements(PrimitiveType primitiveType, uint count, DrawElementsType indexType, long indexOffset);
    ShaderHandle CreateShader(ShaderType glShaderType);
    void ShaderSource(ShaderHandle handle, string[] strings);
    void CompileShader(ShaderHandle handle);
    void GetShader(ShaderHandle handle, ShaderParameterName parameterName, out int i);
    string GetShaderInfoLog(ShaderHandle handle);
    ProgramHandle CreateProgram();
    void AttachShader(ProgramHandle program, ShaderHandle shader);
    void LinkProgram(ProgramHandle program);
    void GetProgram(ProgramHandle program, ProgramPropertyARB programProperty, out int i);
    string GetProgramInfoLog(ProgramHandle program);
    void DetachShader(ProgramHandle program, ShaderHandle shader);
    void DeleteShader(ShaderHandle shader);
    int UniformLocation(ProgramHandle program, string name);
    void ProgramUniformMatrix4(uint programHandle, int location, bool transpose, Matrix4x4 matrix);
    void ProgramUniform(uint programHandle, int location, int value);
    TextureHandle CreateTexture(TextureTarget target);
    void BindTextureUnit(uint unit, TextureHandle handle);
    void DeleteTexture(TextureHandle handle);
    void ObjectLabel<TName>(TName name, string label) where TName :struct, IGlHandle;

    string GetObjectLabel<TName>(TName name) where TName : struct, IGlHandle;
    TextureHandle GenTexture();

    void TextureView(TextureHandle texture, TextureTarget target, TextureHandle origtexture, SizedInternalFormat internalformat,
        uint minlevel, uint numlevels, uint minlayer, uint numlayers);

    void TextureStorage1D(TextureHandle texture, uint levels, SizedInternalFormat internalformat, uint width);
    void TextureStorage2D(TextureHandle texture, uint levels, SizedInternalFormat internalformat, uint width, uint height);
    void TextureStorage3D(TextureHandle texture, uint levels, SizedInternalFormat internalformat, uint width, uint height,
        uint depth);

    void TextureSubImage1D<T0>(TextureHandle texture, int level, int xoffset, uint width, PixelFormat format, PixelType type,
        in T0 pixels)
        where T0 : unmanaged;

    void TextureSubImage2D<T0>(TextureHandle texture, int level, int xoffset, int yoffset, uint width, uint height,
        PixelFormat format, PixelType type, in T0 pixels)
        where T0 : unmanaged;

    void TextureSubImage3D<T0>(TextureHandle texture, int level, int xoffset, int yoffset, int zoffset, uint width, uint height,
        uint depth, PixelFormat format, PixelType type, in T0 pixels)
        where T0 : unmanaged;

    void GenerateTextureMipmap(TextureHandle texture);
}