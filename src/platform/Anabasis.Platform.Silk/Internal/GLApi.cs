using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Anabasis.Platform.Silk.Buffers;
using Anabasis.Platform.Silk.Error;
using Anabasis.Platform.Silk.Shader;
using Anabasis.Platform.Silk.Textures;
using Microsoft.Extensions.Logging;
using Silk.NET.Core.Native;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Internal;

internal class GlApi : IGlApi
{
    private readonly GL             _gl;
    private readonly ILogger<GlApi> _logger;

    public unsafe GlApi(GL gl, ILogger<GlApi> logger) {
        _gl = gl;
        _logger = logger;
        _gl.Enable(EnableCap.DebugOutput);
        _gl.Enable(EnableCap.DebugOutputSynchronous);
        _gl.DebugMessageCallback(DebugCallback, null);
    }

    private void DebugCallback(GLEnum sourceEnum, GLEnum typeEnum, int id, GLEnum severityEnum, int length,
        nint messagePtr, nint userParamPtr) {
        DebugSource source = (DebugSource)sourceEnum;
        DebugType type = (DebugType)typeEnum;
        DebugSeverity severity = (DebugSeverity)severityEnum;
        string? message = SilkMarshal.PtrToString(messagePtr);
        LogLevel level = severity switch {
            DebugSeverity.DontCare => throw new NotImplementedException(),
            DebugSeverity.DebugSeverityNotification => LogLevel.Debug,
            DebugSeverity.DebugSeverityHigh => throw new GlDebugException(source, type, severity, id, message),
            DebugSeverity.DebugSeverityMedium => LogLevel.Warning,
            DebugSeverity.DebugSeverityLow => LogLevel.Information,
            _ => throw new ArgumentOutOfRangeException(nameof(severityEnum), severity, null),
        };
        _logger.GlMessage(level, source, type, id, message);
    }

    public int GetAttribLocation(ProgramHandle program, string name) => _gl.GetAttribLocation(program.Value, name);

    public void BufferData<T0>(BufferTargetARB target, ReadOnlySpan<T0> data, BufferUsageARB usage)
        where T0 : unmanaged {
        _gl.BufferData(target, data, usage);
    }

    public void BufferSubData<T0>(BufferTargetARB target, nint offset, ReadOnlySpan<T0> data)
        where T0 : unmanaged {
        _gl.BufferSubData(target, offset, data);
    }

    public void NamedBufferData<T>(BufferObjectHandle buffer, ReadOnlySpan<T> data, BufferUsageARB usage)
        where T : unmanaged => _gl.NamedBufferData(buffer.Value, (nuint)(data.Length * Marshal.SizeOf<T>()), data,
        (VertexBufferObjectUsage)usage);

    public void NamedBufferSubData<T>(BufferObjectHandle buffer, nint offset, ReadOnlySpan<T> data)
        where T : unmanaged => _gl.NamedBufferSubData(buffer.Value, offset, (nuint)(data.Length * Marshal.SizeOf<T>()),
        data);

    public void EnableVertexArrayAttrib(VertexArrayHandle handle, uint layout) {
        _gl.EnableVertexArrayAttrib(handle.Value, layout);
    }

    public void VertexArrayAttribBinding(VertexArrayHandle vaobj, uint attribindex, SilkBindingIndex bindingindex) {
        _gl.VertexArrayAttribBinding(vaobj.Value, attribindex, bindingindex.Value);
    }

    public BufferObjectHandle CreateBuffer() => new BufferObjectHandle(_gl.CreateBuffer());

    public void DeleteBuffer(BufferObjectHandle handle) {
        _gl.DeleteBuffer(handle.Value);
    }

    public void BindBuffer(BufferTargetARB target, BufferObjectHandle handle) {
        _gl.BindBuffer(target, handle.Value);
    }

    public VertexArrayHandle CreateVertexArray() => new VertexArrayHandle(_gl.CreateVertexArray());

    public void BindVertexArray(VertexArrayHandle handle) {
        _gl.BindVertexArray(handle.Value);
    }

    public void DeleteVertexArray(VertexArrayHandle handle) {
        _gl.DeleteVertexArray(handle.Value);
    }

    public void VertexArrayVertexBuffer(VertexArrayHandle handle, SilkBindingIndex idx, BufferObjectHandle buffer,
        int i, uint sizeOf) {
        _gl.VertexArrayVertexBuffer(handle.Value, idx.Value, buffer.Value, i, sizeOf);
    }

    public void VertexArrayElementBuffer(VertexArrayHandle handle, BufferObjectHandle buffer) {
        _gl.VertexArrayElementBuffer(handle.Value, buffer.Value);
    }

    public void DrawArrays(PrimitiveType primitiveType, int first, uint count) {
        _gl.DrawArrays(primitiveType, first, count);
    }

    public unsafe void DrawElements(PrimitiveType primitiveType, uint count, DrawElementsType indexType,
        long indexOffset) {
        _gl.DrawElements(primitiveType, count, indexType, (void*)indexOffset);
    }

    public ShaderHandle CreateShader(ShaderType glShaderType) => new ShaderHandle(_gl.CreateShader(glShaderType));

    public void ShaderSource(ShaderHandle handle, string[] strings) {
        _gl.ShaderSource(handle.Value, (uint)strings.Length, strings, 0);
    }

    public void CompileShader(ShaderHandle handle) {
        _gl.CompileShader(handle.Value);
    }

    public void GetShader(ShaderHandle handle, ShaderParameterName parameterName, out int i) {
        _gl.GetShader(handle.Value, parameterName, out i);
    }

    public string GetShaderInfoLog(ShaderHandle handle) => _gl.GetShaderInfoLog(handle.Value);

    public ProgramHandle CreateProgram() => new(_gl.CreateProgram());

    public void AttachShader(ProgramHandle program, ShaderHandle shader) {
        _gl.AttachShader(program.Value, shader.Value);
    }

    public void LinkProgram(ProgramHandle program) {
        _gl.LinkProgram(program.Value);
    }

    public void GetProgram(ProgramHandle program, ProgramPropertyARB programProperty, out int i) {
        _gl.GetProgram(program.Value, programProperty, out i);
    }

    public string GetProgramInfoLog(ProgramHandle program) => _gl.GetProgramInfoLog(program.Value);

    public void DetachShader(ProgramHandle program, ShaderHandle shader) {
        _gl.DetachShader(program.Value, shader.Value);
    }

    public void DeleteShader(ShaderHandle shader) {
        _gl.DeleteShader(shader.Value);
    }

    public int UniformLocation(ProgramHandle program, string name) =>
        _gl.GetUniformLocation(program.Value, name);

    public void ProgramUniformMatrix4(uint programHandle, int location, bool transpose, Matrix4x4 matrix) {
        _gl.ProgramUniformMatrix4(programHandle, location, transpose,
            MemoryMarshal.Cast<Matrix4x4, float>(MemoryMarshal.CreateSpan(ref matrix, 1)));
    }

    public void ProgramUniform(uint programHandle, int location, int value) {
        _gl.ProgramUniform1(programHandle, location, value);
    }

    public TextureHandle CreateTexture(TextureTarget target) {
        _gl.CreateTextures(target, 1, out uint value);
        return new TextureHandle(value);
    }

    public void BindTextureUnit(uint unit, TextureHandle handle) {
        _gl.BindTextureUnit(unit, handle.Value);
    }

    public void DeleteTexture(TextureHandle handle) {
        _gl.DeleteTexture(handle.Value);
    }

    public void VertexArrayAttribFormat(VertexArrayHandle vaobj, uint attribindex, int size, VertexAttribType type,
        bool normalized, uint relativeoffset) {
        _gl.VertexArrayAttribFormat(vaobj.Value, attribindex, size, type, normalized, relativeoffset);
    }

    public void Dispose() {
        _gl.Dispose();
    }

    public void GetAndThrowError([CallerMemberName] string caller = null!) {
        ErrorCode error = (ErrorCode)_gl.GetError();
        if (error != ErrorCode.NoError)
            throw new GlException(error, caller, "GL Error caught with glGetError");
    }

    public void ObjectLabel<TName>(TName name, string label)
        where TName : struct, IGlHandle =>
        _gl.ObjectLabel(TName.ObjectType, name.Value, (uint)label.Length, label);

    public string GetObjectLabel<TName>(TName name)
        where TName : struct, IGlHandle {
        _gl.GetInteger(GetPName.MaxLabelLength, out int maxLength);
        _gl.GetObjectLabel(TName.ObjectType, name.Value, (uint)Math.Min(maxLength, 128), out uint _, out string label);
        return label;
    }
}