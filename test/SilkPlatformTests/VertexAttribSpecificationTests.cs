using System.Numerics;
using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Platform.Silk.Internal;
using Anabasis.Platform.Silk.Shader;
using FluentAssertions;
using Moq;
using Silk.NET.OpenGL;

namespace SilkPlatformTests;

public class VertexAttribSpecificationTests
{
    public struct TestVertexType
    {
        [VertexAttribute("pos")]
        public Vector3 Pos;

        [VertexAttribute("texCoord")]
        public Vector2 TexCoord;
    }

    [Fact]
    public void TestAttribListGen() {
        Mock<IGlApi> api = new();
        ProgramHandle handle = new(1);
        api.Setup(it => it.GetAttribLocation(handle, "pos")).Returns(0);
        api.Setup(it => it.GetAttribLocation(handle, "texCoord")).Returns(1);
        SilkShaderSupport support = new();
        support.BuildAttribList<TestVertexType>(api.Object, handle)
            .Should().Equal(new SilkShaderSupport.VertexAttribPointer(0, 3, VertexAttribType.Float, 0),
                new SilkShaderSupport.VertexAttribPointer(1, 2, VertexAttribType.Float, 12));
    }
}