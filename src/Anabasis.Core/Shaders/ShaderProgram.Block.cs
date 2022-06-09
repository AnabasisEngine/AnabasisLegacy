using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Shaders;

public sealed partial class ShaderProgram
{
    private uint GetUniformBlockIndex(string name, ref uint? location) {
        uint index = location ??= Gl.GetUniformBlockIndex(Handle.Value, name);
        Gl.ThrowIfError(nameof(Gl.GetUniformBlockIndex));
        return index;
    }

    public void SetUniformBlockBinding(string name, ref uint? location, uint binding) {
        Gl.UniformBlockBinding(Handle.Value, GetUniformBlockIndex(name, ref location), binding);
        Gl.ThrowIfError(nameof(Gl.UniformBlockBinding));
    }

    public readonly record struct UniformInfo(string Name, UniformType Type, uint Location);

    public unsafe ImmutableDictionary<string,UniformInfo > IntrospectBlock(uint index) {
        Gl.GetProgramResource(Handle.Value, ProgramInterface.UniformBlock, index, 1,
            ProgramResourceProperty.NumActiveVariables, 1, out uint _, out int numUniforms);
        if (numUniforms == 0)
            return ImmutableDictionary<string, UniformInfo>.Empty;
        PooledDictionary<string, UniformInfo> dict = PooledDictionary<string, UniformInfo>.GetInstance();
        Span<int> uniformIndices = new int[numUniforms];
        ReadOnlySpan<ProgramResourceProperty> props = new[] { ProgramResourceProperty.ActiveVariables, };
        Gl.GetProgramResource(Handle.Value, ProgramInterface.UniformBlock, index, props, null, uniformIndices);
        Span<int> values = stackalloc int[3];
        ReadOnlySpan<ProgramResourceProperty> unifProps = new[]
            { ProgramResourceProperty.NameLength, ProgramResourceProperty.Type, ProgramResourceProperty.Location, };
        foreach (int idx in uniformIndices) {
            Gl.GetProgramResource(Handle.Value, ProgramInterface.Uniform, (uint)idx, unifProps, null, values);
            Gl.GetProgramResourceName(Handle.Value, ProgramInterface.Uniform, (uint)idx, (uint)values[0], out uint _,
                out string name);
            dict[name] = new UniformInfo(name, (UniformType)values[1], (uint)values[2]);
        }

        return dict.ToImmutableDictionaryAndFree();
    }
}