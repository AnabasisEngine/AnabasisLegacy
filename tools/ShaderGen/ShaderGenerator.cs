using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ShaderGen.Diagnostics;

namespace ShaderGen;

[Generator]
public class ShaderGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        IncrementalValueProvider<ImmutableDictionary<KnownNamedType, INamedTypeSymbol?>> knownTypesProvider =
            context.CompilationProvider.Select((c, _) => KnownNamedTypes.LoadSymbols(c));
        context.RegisterSourceOutput(context.SyntaxProvider.CreateSyntaxProvider(Predicate, Transform)
                .Collect().Combine(context.CompilationProvider.Combine(knownTypesProvider)),
            (productionContext, tuple) => {
                (ImmutableArray<StructDeclarationSyntax?> syntax,
                    (Compilation compilation, ImmutableDictionary<KnownNamedType, INamedTypeSymbol?> knownTypes)) = tuple;
                if (knownTypes[KnownNamedType.VertexTypeAttribute] == null ||
                    knownTypes[KnownNamedType.VertexAttributeAttribute] == null) {
                    productionContext.ReportDiagnostic(DiagnosticDescriptors.MissingAttributes(Location.None));
                    return;
                }

                foreach (StructDeclarationSyntax? structDeclarationSyntax in syntax) {
                    if (structDeclarationSyntax is null) continue;
                    VertexFormatTypeAnalyzer.GenerateForStruct(
                        compilation.GetSemanticModel(structDeclarationSyntax.SyntaxTree), structDeclarationSyntax,
                        productionContext, knownTypes);
                }
            });
    }

    private static StructDeclarationSyntax? Transform(GeneratorSyntaxContext context, CancellationToken token) {
        SyntaxNode node = context.Node;
        if (node is not StructDeclarationSyntax typeNode) {
            return null;
        }

        SemanticModel model = context.SemanticModel;
        ISymbol? typeSymbol = model.GetDeclaredSymbol(typeNode, token);
        return typeSymbol is not INamedTypeSymbol namedTypeSymbol ? null :
            namedTypeSymbol.GetAttributes().Any(x => x.AttributeClass?.Name == "VertexTypeAttribute") ? typeNode : null;
    }

    private static bool Predicate(SyntaxNode node, CancellationToken _) => node is StructDeclarationSyntax {
        AttributeLists.Count: > 0,
    };
}