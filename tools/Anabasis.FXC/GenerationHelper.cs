using CodeGenHelpers;
using CodeGenHelpers.Internals;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Anabasis.FXC;

internal static class GenerationHelper
{
    public static void Generate(Compilation sourceCompilation, SourceProductionContext context, string name,
        INamespaceSymbol ns, INamedTypeSymbol vertType, INamedTypeSymbol textsType) {
        var codeBuilder = CodeBuilder.Create(ns)
            .AddNamespaceImport("System")
            .AddNamespaceImport("Anabasis.Platform.Abstractions.Shaders")
            .AddNamespaceImport(vertType.ContainingNamespace)
            .AddNamespaceImport(textsType.ContainingNamespace)
            .AddClass($"{name.Substring(1)}Source")
                .AddInterface($"IShaderProgramSource<{name}, {vertType.Name}>")
                .AddProperty("Texts")
                .SetType(textsType)
                .UseGetOnlyAutoProp()
                .AddConstructor()
                    .AddParameter(textsType, "texts")
                    .WithBody(writer => {
                        writer.AppendLine("this.Texts = texts;");
                    });
    }
}