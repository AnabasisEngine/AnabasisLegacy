using FluentAssertions;
using ShaderGen;
using Silk.NET.OpenGL;

namespace ShaderGenTests;

public static class ShaderIntrospectionTests
{
    public const string VertShaderTestData = @"
#version 330 core
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec2 vUv;

out gl_PerVertex
{
  vec4 gl_Position;
};

out vec2 fUv;

void main()
{
    gl_Position = vec4(vPos, 1.0);
    //Setting the uv coordinates on the vertices will mean they get correctly divided out amongst the fragments.
    fUv = vUv;
}
";

    public const string FragShaderTestData = @"
#version 330 core
in vec2 fUv;

//A uniform of the type sampler2D will have the storage value of our texture.
uniform sampler2D uTexture0;

out vec4 FragColor;

void main()
{
    //Here we sample the texture based on the Uv coordinates of the fragment
    FragColor = texture(uTexture0, fUv);
}
";

    [Fact]
    public static void TestUniforms() {
        ShaderGenWindowingContext.CreateWindow(gl => {
            uint program = gl.CreateShaderProgram(ShaderType.FragmentShader, 1, new[] { FragShaderTestData, });
            ShaderIntrospection.UniformInfo[] infos = ShaderIntrospection.QueryUniforms(gl, program).ToArray();
            infos.Should().Contain(new ShaderIntrospection.UniformInfo("uTexture0", UniformType.Sampler2D, 1));
        });
    }

    [Fact]
    public static void TestVertAttribs() {
        ShaderGenWindowingContext.CreateWindow(gl => {
            uint program = gl.CreateShaderProgram(ShaderType.VertexShader, 1, new[] { VertShaderTestData, });
            ShaderIntrospection.VertexAttribInfo[] attribs =
                ShaderIntrospection.QueryVertexAttributes(gl, program).ToArray();
            attribs.Should().Contain(new ShaderIntrospection.VertexAttribInfo[] {
                new("vPos", 1, AttributeType.FloatVec3, 0),
                new("vUv", 1, AttributeType.FloatVec2, 1),
            });
        });
    }
}