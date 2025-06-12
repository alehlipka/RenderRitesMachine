#version 460 core

in vec2 vTexCoord;
in vec3 vNormal;
out vec4 FragColor;

uniform sampler2D uTexture;
uniform vec3 lightDir = normalize(vec3(0.0, -1.0, 0.0));

void main()
{
    vec3 norm = normalize(vNormal);
    float diff = max(dot(norm, -lightDir), 0.2);

    vec4 texColor = texture(uTexture, vTexCoord);
    FragColor = vec4(texColor.rgb * diff, texColor.a);
}
