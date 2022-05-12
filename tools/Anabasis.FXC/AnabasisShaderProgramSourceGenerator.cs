using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Anabasis.FXC;

[Generator]
public class AnabasisShaderProgramSourceGenerator : IIncrementalGenerator
{
    private const string AttributeFullName = "Anabasis.Platform.Abstractions.Shaders.AnabasisShaderProgramAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        // Generate the source using the compilation and enums
        context.RegisterSourceOutput(context
                .SyntaxProvider
                .CreateSyntaxProvider(static (s, _) => IsSyntaxTargetForGeneration(s),
                    static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                .Where(static m => m is not null)
                .Combine(context.CompilationProvider)
                .Combine(context.CompilationProvider.Select((c, _) =>
                    c.GetTypeByMetadataName(AttributeFullName)))
                .Select((tuple, _) => (tuple.Left.Left, tuple.Left.Right, tuple.Right)),
            static (spc, source) =>
                Execute(source.Item2, source.Left!, source.Item3!, spc));
    }

    private static void Execute(Compilation sourceCompilation, ClassDeclarationSyntax sourceClass,
        INamedTypeSymbol attributeSymbol,
        SourceProductionContext spc) {
        SemanticModel semanticModel = sourceCompilation.GetSemanticModel(sourceClass.SyntaxTree);
        if (ModelExtensions.GetDeclaredSymbol(semanticModel, sourceClass) is not INamedTypeSymbol classSymbol) {
            // Something went wrong, bail out.
            return;
        }

        foreach (AttributeData attributeData in classSymbol.GetAttributes()) {
            if (!attributeSymbol.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default))
                continue;

            INamedTypeSymbol vertType = (INamedTypeSymbol)attributeData.ConstructorArguments[0].Value!;
            INamedTypeSymbol textsType = (INamedTypeSymbol)attributeData.ConstructorArguments[1].Value!;

            GenerationHelper.Generate(sourceCompilation, spc, classSymbol.Name,
                classSymbol.ContainingNamespace, vertType, textsType);
        }
    }

    private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context) {
        ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        foreach (AttributeListSyntax attributeList in classDeclarationSyntax.AttributeLists) {
            foreach (AttributeSyntax attribute in attributeList.Attributes) {
                if (ModelExtensions.GetSymbolInfo(context.SemanticModel, attribute).Symbol is not IMethodSymbol attributeSymbol)
                    continue;
                INamedTypeSymbol typeSymbol = attributeSymbol.ContainingType;
                string fullName = typeSymbol.ToDisplayString();
                if (fullName == AttributeFullName) {
                    return classDeclarationSyntax;
                }
            }
        }

        return null;
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) =>
        node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
}