#version 450 core
out vec4 FragColor;

in vec3 fColor;
in vec2 fTexCoord;
in float fTexLayer;

uniform sampler2DArray texarray;

void main()
{
    FragColor = vec4(fColor, 1.0)/* * texture(texarray, vec3(fTexCoord, fTexLayer))*/;
}