using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;
using ShaderGen.Diagnostics;
using Silk.NET.OpenGL;

namespace ShaderGen;

public static class VertexFormatTypeAnalyzer
{
    public struct VertexTypeField
    {
        public VertexTypeField(bool normalize, int offset, VertexAttribType type, string name) {
            Normalize = normalize;
            Offset = offset;
            Type = type;
            Name = name;
        }

        public string Name { get; }
        public VertexAttribType Type { get; }
        public int Offset { get; }
        public bool Normalize { get; }
    }

    public static void GenerateForStruct(SemanticModel semanticModel, StructDeclarationSyntax typeDeclSyntax,
        SourceProductionContext context, ImmutableDictionary<string, AdditionalText> additionals,
        ImmutableDictionary<KnownNamedType, INamedTypeSymbol?> knownTypes) {
        INamedTypeSymbol vertexTypeAttribute = knownTypes[KnownNamedType.VertexTypeAttribute]!;
        INamedTypeSymbol vertexAttributeAttribute = knownTypes[KnownNamedType.VertexAttributeAttribute]!;
        INamedTypeSymbol structLayoutAttribute = knownTypes[KnownNamedType.StructLayoutAttribute]!;
        INamedTypeSymbol fieldOffsetAttribute = knownTypes[KnownNamedType.FieldOffsetAttribute]!;

        bool canGenerate = true;

        if (semanticModel.GetDeclaredSymbol(typeDeclSyntax, context.CancellationToken) is not { } typeSymbol) {
            return;
        }

        AttributeData? vertTypeData = typeSymbol.GetAttributes()
            .SingleOrDefault(a => vertexTypeAttribute.Equals(a.AttributeClass, SymbolEqualityComparer.Default));
        if (vertTypeData is null)
            return;
        AttributeData? structLayoutData = typeSymbol.GetAttributes().SingleOrDefault(a =>
            structLayoutAttribute.Equals(a.AttributeClass, SymbolEqualityComparer.Default));

        if (structLayoutData is null) {
            context.ReportDiagnostic(DiagnosticDescriptors.MissingTypeAttributes(typeDeclSyntax.GetLocation(),
                typeDeclSyntax.ToString()));
            return;
        }

        if (!typeDeclSyntax.Modifiers.Any(SyntaxKind.PartialKeyword)) {
            context.ReportDiagnostic(DiagnosticDescriptors.TargetStructIsNotPartial(typeDeclSyntax.GetLocation(),
                typeDeclSyntax.Identifier.ToString()));
            canGenerate = false;
        }

        bool isSequential = (LayoutKind)structLayoutData.ConstructorArguments[0].Value! == LayoutKind.Sequential;
        int cumulOffset = 0;
        List<VertexTypeField> fields = new();
        foreach (MemberDeclarationSyntax syntax in typeDeclSyntax.Members) {
            ISymbol? member = semanticModel.GetDeclaredSymbol(syntax);
            if (member is not IFieldSymbol field)
                continue;
            if (!field.Type.IsUnmanagedType) {
                canGenerate = false;
                continue;
            }

            AttributeData? attributeData = field.GetAttributes().SingleOrDefault(a =>
                vertexAttributeAttribute.Equals(a.AttributeClass, SymbolEqualityComparer.Default));
            AttributeData? fieldOffsetData = field.GetAttributes().SingleOrDefault(a =>
                fieldOffsetAttribute.Equals(a.AttributeClass, SymbolEqualityComparer.Default));

            if (attributeData is null || (!isSequential && fieldOffsetData is null)) {
                context.ReportDiagnostic(DiagnosticDescriptors.MissingFieldAttributes(syntax.GetLocation(),
                    typeDeclSyntax.Identifier.ToString(), field.Name));
                canGenerate = false;
                continue;
            }

            bool normalize = (bool)attributeData.ConstructorArguments[1].Value!;
            int offset = isSequential ? cumulOffset : (int)fieldOffsetData!.ConstructorArguments[0].Value!;
            if (isSequential)
                cumulOffset += GetSize(field, knownTypes, field.Type);

            fields.Add(new VertexTypeField(normalize, offset,
                GetVertexAttribType((INamedTypeSymbol)field.Type, knownTypes),
                (string)attributeData.ConstructorArguments[0].Value!));
        }

        ImmutableDictionary<string, VertexTypeField> named = fields.ToImmutableDictionary(p => p.Name);

        if (!canGenerate) return;

        string vertFile = (string)vertTypeData.ConstructorArguments[0].Value!;
        int divisor = (int)vertTypeData.ConstructorArguments[1].Value!;
        ShaderGenWindowingContext.CreateWindow(gl => {
            ArrayBuilder<VertexFormatGenerator.VertexAttribPointer> pointers =
                ArrayBuilder<VertexFormatGenerator.VertexAttribPointer>.GetInstance();
            uint program = gl.CreateShaderProgram(ShaderType.VertexShader, 1,
                new[] { additionals[vertFile].GetText()!.ToString(), });
            foreach (ShaderIntrospection.VertexAttribInfo info in ShaderIntrospection.QueryVertexAttributes(gl,
                         program)) {
                VertexTypeField field = named[info.Name];
                pointers.Add(new VertexFormatGenerator.VertexAttribPointer {
                    Info = info,
                    Normalize = field.Normalize,
                    Offset = field.Offset,
                    Type = field.Type,
                });
            }

            NamespaceDeclarationSyntax syntax = SyntaxFactory
                .NamespaceDeclaration(SyntaxFactory.ParseName(typeSymbol.ContainingNamespace.Name))
                .AddMembers(VertexFormatGenerator.VertexFormat(typeSymbol.Name, typeSymbol.DeclaredAccessibility,
                    pointers.ToArrayAndFree(), divisor));
            context.AddSource($"{typeSymbol.Name}.Format.cs", syntax.GetText());
        });
    }

    private static VertexAttribType GetVertexAttribType(INamedTypeSymbol type,
        ImmutableDictionary<KnownNamedType, INamedTypeSymbol?> compilation) =>
        type.SpecialType
            switch {
                SpecialType.System_Boolean => VertexAttribType.UnsignedByte,
                SpecialType.System_Byte => VertexAttribType.UnsignedByte,
                SpecialType.System_Double => VertexAttribType.Double,
                SpecialType.System_Single => VertexAttribType.Float,
                SpecialType.System_Int16 => VertexAttribType.Short,
                SpecialType.System_Int32 => VertexAttribType.Int,
                SpecialType.System_UInt16 => VertexAttribType.UnsignedShort,
                SpecialType.System_UInt32 => VertexAttribType.UnsignedInt,
                SpecialType.None => GetOtherTypeDescriptor(type, compilation),
                _ => throw new InvalidOperationException()
            };

    private static VertexAttribType GetOtherTypeDescriptor(INamedTypeSymbol type,
        ImmutableDictionary<KnownNamedType, INamedTypeSymbol?> compilation) {
        if (type.Equals(compilation[KnownNamedType.Half], SymbolEqualityComparer.Default))
            return VertexAttribType.HalfFloat;
        if (type.Equals(compilation[KnownNamedType.NumericsVector2], SymbolEqualityComparer.Default) ||
            type.Equals(compilation[KnownNamedType.NumericsVector3], SymbolEqualityComparer.Default))
            return VertexAttribType.Float;
        if (type.Arity != 1)
            throw new InvalidOperationException();
        INamedTypeSymbol typeArgument = (INamedTypeSymbol)type.TypeArguments[0];
        if (type.Equals(compilation[KnownNamedType.SilkVector2]?.Construct(typeArgument),
                SymbolEqualityComparer.Default) || type.Equals(
                compilation[KnownNamedType.SilkVector3]?.Construct(typeArgument),
                SymbolEqualityComparer.Default) || type.Equals(
                compilation[KnownNamedType.SilkVector4]?.Construct(typeArgument),
                SymbolEqualityComparer.Default)) {
            return GetVertexAttribType(typeArgument, compilation);
        }

        throw new InvalidOperationException();
    }

    private static int GetSize(IFieldSymbol field, ImmutableDictionary<KnownNamedType, INamedTypeSymbol?> compilation,
        ITypeSymbol type) =>
        type.SpecialType switch {
            SpecialType.System_Boolean => sizeof(bool),
            SpecialType.System_Byte => sizeof(byte),
            SpecialType.System_Double => sizeof(double),
            SpecialType.System_Single => sizeof(float),
            SpecialType.System_Int16 => sizeof(short),
            SpecialType.System_Int32 => sizeof(int),
            SpecialType.System_UInt16 => sizeof(ushort),
            SpecialType.System_UInt32 => sizeof(uint),
            SpecialType.None => GetOtherTypeSize(field, (INamedTypeSymbol)type, compilation) *
                                (field.IsFixedSizeBuffer ? field.FixedSize : 1),
            _ => throw new InvalidOperationException()
        };

    private static unsafe int GetOtherTypeSize(IFieldSymbol field, INamedTypeSymbol type,
        ImmutableDictionary<KnownNamedType, INamedTypeSymbol?> compilation) {
        if (type.Equals(compilation[KnownNamedType.Half], SymbolEqualityComparer.Default))
            return sizeof(Half);
        if (type.Equals(compilation[KnownNamedType.NumericsVector2], SymbolEqualityComparer.Default) ||
            type.Equals(compilation[KnownNamedType.NumericsVector3], SymbolEqualityComparer.Default))
            return sizeof(float);
        if (type.Arity != 1)
            throw new InvalidOperationException();
        INamedTypeSymbol typeArgument = (INamedTypeSymbol)type.TypeArguments[0];
        if (type.Equals(compilation[KnownNamedType.SilkVector2]?.Construct(typeArgument),
                SymbolEqualityComparer.Default)) {
            return 2 * GetSize(field, compilation, typeArgument);
        }

        if (type.Equals(compilation[KnownNamedType.SilkVector3]?.Construct(typeArgument),
                SymbolEqualityComparer.Default)) {
            return 3 * GetSize(field, compilation, typeArgument);
        }

        if (type.Equals(compilation[KnownNamedType.SilkVector4]?.Construct(typeArgument),
                SymbolEqualityComparer.Default)) {
            return 4 * GetSize(field, compilation, typeArgument);
        }

        throw new InvalidOperationException();
    }
}