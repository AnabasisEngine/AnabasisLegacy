using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using ShaderGen;
using Silk.NET.OpenGL;

namespace ShaderGenTests;

[UsesVerify]
public static class FormatMethodGenTests
{
    public static TheoryData<VertexFormatGenerator.VertexAttribPointer[], int> Data = new() {
        {
            new[] {
                new VertexFormatGenerator.VertexAttribPointer {
                    Info = new ShaderIntrospection.VertexAttribInfo("vPos", 1, AttributeType.FloatVec3, 0),
                    Normalize = false,
                    Offset = 0,
                    Type = VertexAttribType.Float,
                },
                new VertexFormatGenerator.VertexAttribPointer {
                    Info = new ShaderIntrospection.VertexAttribInfo("vUv", 1, AttributeType.FloatVec2, 1),
                    Normalize = false,
                    Offset = 12,
                    Type = VertexAttribType.Float,
                },
            },
            0
        },
    };

    [Theory, MemberData(nameof(Data)),]
    public static async Task TestFormatterMethodGen(VertexFormatGenerator.VertexAttribPointer[] pointers, int divisor) {
        CSharpSyntaxNode node = VertexFormatGenerator.CreateVertexFormatMethod
            (pointers, divisor);

        AdhocWorkspace workspace = new();
        workspace.AddSolution(
            SolutionInfo.Create(SolutionId.CreateNewId("formatter"),
                VersionStamp.Default)
        );

        node = (CSharpSyntaxNode)Formatter.Format(node, workspace);
        SyntaxTree tree = CSharpSyntaxTree.Create(node);
        await Verify((await tree.GetTextAsync()).ToString())
            .UseDirectory("Snapshots")
            .UseParameters(pointers, divisor);
    }

    [Theory, MemberData(nameof(Data)),]
    public static async Task TestFormatterStructGen(VertexFormatGenerator.VertexAttribPointer[] pointers, int divisor) {
        CSharpSyntaxNode node =
            VertexFormatGenerator.VertexFormat("Vertex", Accessibility.Public, pointers, divisor);

        AdhocWorkspace workspace = new();
        workspace.AddSolution(
            SolutionInfo.Create(SolutionId.CreateNewId("formatter"),
                VersionStamp.Default)
        );

        node = (CSharpSyntaxNode)Formatter.Format(node, workspace);
        SyntaxTree tree = CSharpSyntaxTree.Create(node);
        await Verify((await tree.GetTextAsync()).ToString())
            .UseDirectory("Snapshots")
            .UseParameters(pointers, divisor);
    }
}