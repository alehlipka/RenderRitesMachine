#version 460 core

in vec3 vNormal;
in vec3 vFragPos;
in vec2 vTexCoord;

out vec4 FragColor;

uniform sampler2D uAlbedo;
uniform vec3 lightDir = normalize(vec3(-0.5, -1.0, -0.2));
uniform vec3 lightColor = vec3(1.0, 0.9, 0.8);
uniform vec3 ambientColor = vec3(0.4, 0.5, 0.8);

void main()
{
    float NdotL = dot(vNormal, -lightDir);
    float toon = floor(NdotL * 3.0) / 3.0;
    toon = max(toon, 0.2);

    vec3 albedo = texture(uAlbedo, vTexCoord).rgb;
    vec3 diffuse = albedo * lightColor * toon;

    vec3 ambient = ambientColor * albedo * 0.3;

    vec3 viewDir = normalize(-vFragPos);
    vec3 reflectDir = reflect(lightDir, vNormal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0);
    vec3 specular = lightColor * spec * 0.5;

    FragColor = vec4(diffuse + ambient + specular, 1.0);
}
