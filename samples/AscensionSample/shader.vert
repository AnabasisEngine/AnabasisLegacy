#version 330 core
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec2 vUv;

out gl_PerVertex
{
    vec4 gl_Position;
};

out v_out
{
    vec2 fUv;
} v_out;

void main()
{
    gl_Position = vec4(vPos, 1.0);
    //Setting the uv coordinates on the vertices will mean they get correctly divided out amongst the fragments.
    v_out.fUv = vUv;
}