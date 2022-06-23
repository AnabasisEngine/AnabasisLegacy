using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Shaders;

public sealed partial class ShaderProgram
{
    private uint GetUniformBlockIndex(string name, ref uint? location) {
        uint index = location ??= Api.GetUniformBlockIndex(Handle.Value, name);
        Api.ThrowIfError(nameof(Api.GetUniformBlockIndex));
        return index;
    }

    public void SetUniformBlockBinding(string name, ref uint? location, uint binding) {
        Api.UniformBlockBinding(Handle.Value, GetUniformBlockIndex(name, ref location), binding);
        Api.ThrowIfError(nameof(Api.UniformBlockBinding));
    }

    public readonly record struct UniformInfo(string Name, UniformType Type, uint Location);

    public unsafe ImmutableDictionary<string, UniformInfo> IntrospectBlock(uint index) {
        Api.GetProgramResource(Handle.Value, ProgramInterface.UniformBlock, index, 1,
            ProgramResourceProperty.NumActiveVariables, 1, out uint _, out int numUniforms);
        if (numUniforms == 0)
            return ImmutableDictionary<string, UniformInfo>.Empty;
        PooledDictionary<string, UniformInfo> dict = PooledDictionary<string, UniformInfo>.GetInstance();
        Span<int> uniformIndices = stackalloc int[numUniforms];
        ReadOnlySpan<ProgramResourceProperty> props = stackalloc ProgramResourceProperty[]
            { ProgramResourceProperty.ActiveVariables, };
        Api.GetProgramResource(Handle.Value, ProgramInterface.UniformBlock, index, props, null, uniformIndices);
        Span<int> values = stackalloc int[3];
        ReadOnlySpan<ProgramResourceProperty> unifProps = stackalloc ProgramResourceProperty[]
            { ProgramResourceProperty.NameLength, ProgramResourceProperty.Type, ProgramResourceProperty.Location, };
        foreach (int idx in uniformIndices) {
            Api.GetProgramResource(Handle.Value, ProgramInterface.Uniform, (uint)idx, unifProps, null, values);
            Api.GetProgramResourceName(Handle.Value, ProgramInterface.Uniform, (uint)idx, (uint)values[0], out uint _,
                out string name);
            dict[name] = new UniformInfo(name, (UniformType)values[1], (uint)values[2]);
        }

        return dict.ToImmutableDictionaryAndFree();
    }
}