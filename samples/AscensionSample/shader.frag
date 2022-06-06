#version 330 core


in v_out
{
    vec2 fUv;
} v_out;

//A uniform of the type sampler2D will have the storage value of our texture.
uniform sampler2D uTexture0;

out vec4 FragColor;

void main()
{
    //Here we sample the texture based on the Uv coordinates of the fragment
    FragColor = texture(uTexture0, v_out.fUv);
}