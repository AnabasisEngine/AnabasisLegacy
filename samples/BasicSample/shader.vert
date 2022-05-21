#version 430 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec3 aColor;
layout (location = 2) in vec2 aOffset;

out vec3 fColor;

void main()
{
    vec2 pos = aPos / 10.0 * (gl_InstanceID / 75.0);
    fColor = aColor;
    gl_Position = vec4(pos + aOffset, 0.0, 1.0);
}