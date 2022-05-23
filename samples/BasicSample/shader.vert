#version 430 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec3 aColor;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in vec2 aOffset;
layout (location = 4) in int aTexLayer;

out vec3 fColor;
out vec2 fTexCoord;
out int fTexLayer;

void main()
{
    vec2 pos = aPos / 8.0 * (gl_InstanceID / 100.0);
    fColor = aColor;
    fTexCoord = aTexCoord;
    fTexLayer = aTexLayer;
    gl_Position = vec4(pos + aOffset, 0.0, 1.0);
}