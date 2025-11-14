#version 460 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec3 aTexture;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 cameraPosition;
uniform float time;
uniform float outlineThickness = 5;
uniform vec2 viewportSize = vec2(1920.0, 1080.0);

void main()
{
    vec4 clipPosition = vec4(aPosition, 1.0) * model * view * projection;
    
    vec3 viewNormal = normalize(vec3(vec4(aNormal, 0.0) * model * view));
    
    vec3 outlineOffset = viewNormal * outlineThickness;
    
    vec4 clipOffset = vec4(outlineOffset, 0.0) * projection;
    
    vec2 ndcOffset = clipOffset.xy / clipPosition.w;
    
    float ndcScale = outlineThickness * 6 / viewportSize.y;
    vec2 ndcScaledOffset = ndcOffset * ndcScale;
    
    vec2 clipScaledOffset = ndcScaledOffset * clipPosition.w;
    
    clipPosition.xy += clipScaledOffset;
    
    gl_Position = clipPosition;
}
