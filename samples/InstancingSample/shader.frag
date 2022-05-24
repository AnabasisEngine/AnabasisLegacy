#version 450 core
out vec4 FragColor;

in vec4 fColor;
//in vec2 fTexCoord;
//in float fTexLayer;

//uniform sampler2DArray texarray;

void main()
{
    FragColor = fColor/* * texture(texarray, vec3(fTexCoord, fTexLayer))*/;
}