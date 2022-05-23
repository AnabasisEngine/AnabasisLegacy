using System.Diagnostics.CodeAnalysis;
using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Graphics.Abstractions.Internal;
using Anabasis.Platform.Abstractions;
using Anabasis.Platform.Abstractions.Resources;

namespace Anabasis.Graphics.Abstractions.Shaders;

public abstract class ShaderProgram : IShaderProgramTexts, IPlatformResource
{
    protected ShaderProgram(IShaderSupport support) {
        Support = support;
    }

    protected IShaderSupport Support { get; }

    [MemberNotNull(nameof(Handle))]
    public async ValueTask CompileAsync(CancellationToken cancellationToken = default) {
        Handle = await Support.CompileAndLinkAsync(this, cancellationToken);
    }

    public IPlatformHandle Handle { get; private set; } = null!;

    public IDisposable Use() {
        Support.UseShaderProgram(Handle);
        return new GenericDisposer(() => Support.UseShaderProgram(Support.NullHandle));
    }

    public abstract IEnumerable<(ShaderType, Task<string>)> GetTexts();

    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            Support.DisposeProgram(Handle);
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected IShaderParameter<T> CreateParameter<T>(ref IShaderParameter<T>? parameter, string name) {
        return parameter ??= Support.CreateParameter<T>(name, Handle);
    }

    private readonly Dictionary<Type, IVertexBufferFormatter> _formatters = new();

    public void FormatBuffer<T>(IVertexArray vertexArray, IBufferObject<T> buffer,
        IVertexBufferFormatter<T>? formatter)
        where T : unmanaged {
        if (formatter != null) {
            _formatters[typeof(T)] = formatter;
        } else if (_formatters.TryGetValue(typeof(T), out IVertexBufferFormatter? f) &&
                   f is IVertexBufferFormatter<T> fmt) {
            formatter = fmt;
        } else {
            _formatters[typeof(T)] = formatter = Support.CreateVertexFormatter<T>(Handle);
        }

        formatter.BindVertexFormat(vertexArray, buffer);
    }
}