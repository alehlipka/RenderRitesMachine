#version 460 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec3 aTexture;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 cameraPosition;
uniform float time;
uniform float outlineThickness = 0.1;

void main()
{
    vec3 worldPosition = vec3(vec4(aPosition, 1.0) * model);
    float distanceToCamera = length(cameraPosition - worldPosition);
    float adjustedThickness = outlineThickness * (1.0 + distanceToCamera * 0.01);
    adjustedThickness *= (1.0 + 0.1 * sin(time * 2.0));

    vec3 outlinePosition = aPosition + aNormal * adjustedThickness;
    gl_Position = vec4(outlinePosition, 1.0) * model * view * projection;
}
