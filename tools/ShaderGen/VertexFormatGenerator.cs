using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Silk.NET.OpenGL;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ShaderGen;

public static class VertexFormatGenerator
{
    private const string FormatterParamList =
        "(Anabasis.Core.Rendering.VertexArrayBindingIndex bindingIndex, Silk.NET.OpenGL.GL gl, Anabasis.Core.Handles.VertexArrayHandle handle)";

    private static InvocationExpressionSyntax EnableVertexArrayAttrib(int layout) => InvocationExpression(
        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("gl"),
                IdentifierName("EnableVertexArrayAttrib")), ArgumentList(
            SeparatedList(new[] {
                Argument(IdentifierName("id")),
                NumericLiteralArgument(layout),
            })));

    private static ArgumentSyntax NumericLiteralArgument(int layout) =>
        Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression,
            Literal(layout)));

    private static ArgumentSyntax BoolLiteralArgument(bool value) => Argument(
        LiteralExpression(value ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression));

    private static InvocationExpressionSyntax AttribFormat(int layout, int count, VertexAttribType attribType,
        bool normalize, int offset) => InvocationExpression(MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression, IdentifierName("gl"),
        IdentifierName("VertexArrayAttribFormat")), ArgumentList(
        SeparatedList(new[] {
            Argument(IdentifierName("id")),
            NumericLiteralArgument(layout),
            NumericLiteralArgument(count),
            EnumLiteralArgument(attribType),
            BoolLiteralArgument(normalize),
            NumericLiteralArgument(offset),
        })));

    private static InvocationExpressionSyntax AttribBinding(int layout) => InvocationExpression(
        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName("gl"),
            IdentifierName("VertexArrayAttribBinding")), ArgumentList(
            SeparatedList(new[] {
                Argument(IdentifierName("id")),
                NumericLiteralArgument(layout),
                Argument(IdentifierName("idx")),
            })
        ));

    private static InvocationExpressionSyntax AttribDivisor(int divisor) => InvocationExpression(
        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName("gl"),
            IdentifierName("VertexArrayBindingDivisor")), ArgumentList(
            SeparatedList(new[] {
                Argument(IdentifierName("id")),
                Argument(IdentifierName("idx")),
                NumericLiteralArgument(divisor),
            })));

    public struct VertexAttribPointer
        : IEquatable<VertexAttribPointer>
    {
        public ShaderIntrospection.VertexAttribInfo Info;
        public bool                                 Normalize;
        public int                                  Offset;
        public VertexAttribType                     Type;

        public bool Equals(VertexAttribPointer other) => Info.Equals(other.Info) && Normalize == other.Normalize &&
                                                         Offset == other.Offset && Type == other.Type;

        public override bool Equals(object? obj) => obj is VertexAttribPointer other && Equals(other);

        public override int GetHashCode() {
            unchecked {
                int hashCode = Info.GetHashCode();
                hashCode = (hashCode * 397) ^ Normalize.GetHashCode();
                hashCode = (hashCode * 397) ^ Offset;
                hashCode = (hashCode * 397) ^ (int)Type;
                return hashCode;
            }
        }

        public override string ToString() =>
            $"{nameof(Info)}: {Info}, {nameof(Normalize)}: {Normalize}, {nameof(Offset)}: {Offset}, {nameof(Type)}: {Type}";
    }

    private static ArgumentSyntax EnumLiteralArgument(Enum attribType) =>
        Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
            ParseTypeName(attribType.GetType().FullName),
            IdentifierName(attribType.ToString())));

    public static MethodDeclarationSyntax CreateVertexFormatMethod(VertexAttribPointer[] attribInfos, int divisor) {
        MethodDeclarationSyntax methodDeclarationSyntax = MethodDeclaration(ParseTypeName("void"), "EstablishVertexFormat")
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
            .WithParameterList(ParseParameterList(FormatterParamList));
        methodDeclarationSyntax = methodDeclarationSyntax.AddBodyStatements(LocalDeclarationStatement(
            VariableDeclaration(ParseTypeName("uint")).AddVariables(VariableDeclarator("id")
                    .WithInitializer(EqualsValueClause(MemberAccessExpression(SyntaxKind
                            .SimpleMemberAccessExpression, IdentifierName("handle"),
                        IdentifierName("Value")))))
                .AddVariables(VariableDeclarator("idx")
                    .WithInitializer(EqualsValueClause(MemberAccessExpression(SyntaxKind
                            .SimpleMemberAccessExpression, IdentifierName("bindingIndex"),
                        IdentifierName("Value")))))));
        foreach (VertexAttribPointer pointer in attribInfos) {
            DeterminePointerTypeFromAttribute(pointer.Info.PointerType, out int count);
            for (int i = 0; i < pointer.Info.Count; i++) {
                methodDeclarationSyntax = methodDeclarationSyntax.AddBodyStatements(
                    ExpressionStatement(EnableVertexArrayAttrib(pointer.Info.Location)),
                    ExpressionStatement(AttribFormat(pointer.Info.Location, count, pointer.Type,
                        pointer.Normalize, pointer.Offset + i)),
                    ExpressionStatement(AttribBinding(pointer.Info.Location)));
            }
        }

        methodDeclarationSyntax =
            methodDeclarationSyntax.AddBodyStatements(ExpressionStatement(AttribDivisor(divisor)));
        return methodDeclarationSyntax;
    }

    private static void DeterminePointerTypeFromAttribute(AttributeType infoPointerType, out int i) {
        switch (infoPointerType) {
            case AttributeType.Int:
            case AttributeType.UnsignedInt:
            case AttributeType.Float:
            case AttributeType.Double:
            case AttributeType.Bool:
                i = 1;
                break;
            case AttributeType.FloatVec2:
                i = 2;
                break;
            case AttributeType.FloatVec3:
                i = 3;
                break;
            case AttributeType.FloatVec4:
                i = 4;
                break;
            case AttributeType.IntVec2:
                i = 2;
                break;
            case AttributeType.IntVec3:
                i = 3;
                break;
            case AttributeType.IntVec4:
                i = 4;
                break;
            case AttributeType.BoolVec2:
                i = 2;
                break;
            case AttributeType.BoolVec3:
                i = 3;
                break;
            case AttributeType.BoolVec4:
                i = 4;
                break;
            case AttributeType.UnsignedIntVec2:
                i = 2;
                break;
            case AttributeType.UnsignedIntVec3:
                i = 3;
                break;
            case AttributeType.UnsignedIntVec4:
                i = 4;
                break;
            case AttributeType.DoubleVec2:
                i = 2;
                break;
            case AttributeType.DoubleVec3:
                i = 3;
                break;
            case AttributeType.DoubleVec4:
                i = 4;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(infoPointerType), infoPointerType, null);
        }
    }

    public static StructDeclarationSyntax VertexFormat(string name, Accessibility accessibility,
        VertexAttribPointer[] attribInfos, int divisor) => StructDeclaration(name)
        .AddModifiers(Token(accessibility switch {
            Accessibility.Internal => SyntaxKind.InternalKeyword,
            Accessibility.Public => SyntaxKind.PublicKeyword,
            _ => throw new ArgumentOutOfRangeException(nameof(accessibility), accessibility, null),
        }), Token(SyntaxKind.PartialKeyword))
        .AddBaseListTypes(
            SimpleBaseType(ParseTypeName("Anabasis.Core.Rendering.IVertexType")))
        .AddMembers(CreateVertexFormatMethod(attribInfos, divisor));
}