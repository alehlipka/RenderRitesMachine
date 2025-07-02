#version 460 core

in vec2 TexCoords;
out vec4 color;

uniform sampler2D text;
uniform vec3 textColor;
uniform float edgeValue = 0.5;
uniform float smoothing = 0.05;

void main()
{
    float distance = texture(text, TexCoords).r;
    float alpha = smoothstep(edgeValue - smoothing, edgeValue + smoothing, distance);
    color = vec4(textColor, alpha);
}
