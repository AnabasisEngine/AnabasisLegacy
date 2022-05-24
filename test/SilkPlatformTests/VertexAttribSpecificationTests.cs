using System.Numerics;
using System.Reflection;
using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Platform.Silk.Internal;
using Anabasis.Platform.Silk.Shader;
using FluentAssertions;
using Moq;
using Silk.NET.OpenGL;
using static Anabasis.Platform.Silk.Shader.SilkShaderSupport;

namespace SilkPlatformTests;

public class VertexAttribSpecificationTests
{
    public struct TestVertexType
    {
        [VertexAttribute("pos")]
        public Vector3 Pos;

        [VertexAttribute("texCoord")]
        public Vector2 TexCoord;

        [VertexAttribute("color", Normalize = true)]
        public Color Color;
    }

    [Theory, MemberData(nameof(AttribListGenData))]
    internal void TestAttribListGen(Type type, Dictionary<string, int> attribLocs,
        VertexAttribPointer[] pointers) {
        Mock<IGlApi> api = new();
        ProgramHandle handle = new(1);
        foreach ((string name, int value) in attribLocs) {
            api.Setup(it => it.GetAttribLocation(handle, name)).Returns(value);
        }

        MethodInfo method = typeof(SilkShaderSupport).GetMethod(nameof(BuildAttribList),
            BindingFlags.NonPublic | BindingFlags.Static)!;
        MethodInfo genericMethod = method.MakeGenericMethod(type);
        object? result = genericMethod.Invoke(null, new object[] { api.Object, handle, });
        result.As<IEnumerable<VertexAttribPointer>>()
            .Should().Equal(pointers);
    }

    internal static TheoryData<Type, Dictionary<string, int>, VertexAttribPointer[]>
        AttribListGenData() => new() {
        {
            typeof(TestVertexType), new Dictionary<string, int> { { "pos", 0 }, { "texCoord", 1 }, { "color", 2 } },
            new[] {
                new VertexAttribPointer(0, 3, VertexAttribType.Float, 0, false),
                new VertexAttribPointer(1, 2, VertexAttribType.Float, 12, false),
                new VertexAttribPointer(2, 4, VertexAttribType.Byte, 20, true),
            }
        },
    };
}