using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Silk.NET.OpenGL;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ShaderGen;

public static class VertexFormatGenerator
{
    private const string FormatterParamList =
        "(VertexArrayBindingIndex bindingIndex, VertexFormatter formatter, VertexArrayHandle handle)";

    private static ArgumentSyntax NumericLiteralArgument(int layout) =>
        Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression,
            Literal(layout)));

    private static ArgumentSyntax BoolLiteralArgument(bool value) => Argument(
        LiteralExpression(value ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression));

    private static InvocationExpressionSyntax AttribFormatV2(int layout, int count, VertexAttribType attribType,
        bool normalize, int offset) => InvocationExpression(MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression, IdentifierName("formatter"),
        IdentifierName("WriteVertexArrayAttribFormat")), ArgumentList(SeparatedList<ArgumentSyntax>()
        .Add(Argument(IdentifierName("handle")))
        .Add(Argument(IdentifierName("bindingIndex")))
        .Add(NumericLiteralArgument(layout))
        .Add(NumericLiteralArgument(count))
        .Add(EnumLiteralArgument(attribType))
        .Add(BoolLiteralArgument(normalize))
        .Add(NumericLiteralArgument(offset))));

    private static InvocationExpressionSyntax AttribDivisor(int divisor) => InvocationExpression(
        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName("formatter"),
            IdentifierName("WriteVertexArrayBindingDivisor")), ArgumentList(
            SeparatedList(new[] {
                Argument(IdentifierName("handle")),
                Argument(IdentifierName("bindingIndex")),
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
            ParseTypeName(attribType.GetType().Name),
            IdentifierName(attribType.ToString())));

    public static MethodDeclarationSyntax CreateVertexFormatMethod(VertexAttribPointer[] attribInfos, int divisor) {
        MethodDeclarationSyntax methodDeclarationSyntax =
            MethodDeclaration(ParseTypeName("void"), "EstablishVertexFormat")
                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                .WithParameterList(ParseParameterList(FormatterParamList));
        methodDeclarationSyntax = methodDeclarationSyntax
            .AddBodyStatements(MethodDeclarationSyntax(attribInfos).ToArray())
            .AddBodyStatements(ExpressionStatement(AttribDivisor(divisor)));
        return methodDeclarationSyntax;
    }

    private static IEnumerable<StatementSyntax> MethodDeclarationSyntax(VertexAttribPointer[] attribInfos) {
        foreach (VertexAttribPointer pointer in attribInfos) {
            DeterminePointerTypeFromAttribute(pointer.Info.PointerType, out int count);
            for (int i = 0; i < pointer.Info.Count; i++) {
                yield return ExpressionStatement(AttribFormatV2(pointer.Info.Location, count, pointer.Type,
                    pointer.Normalize, pointer.Offset + i));
            }
        }
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
            SimpleBaseType(ParseTypeName("IVertexType")))
        .AddMembers(CreateVertexFormatMethod(attribInfos, divisor));
}