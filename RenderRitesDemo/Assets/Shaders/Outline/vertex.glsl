#version 460 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec3 aTexture;

layout (location = 3) in vec4 instanceModel0;
layout (location = 4) in vec4 instanceModel1;
layout (location = 5) in vec4 instanceModel2;
layout (location = 6) in vec4 instanceModel3;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 cameraPosition;
uniform float time;
uniform float outlineThickness = 5;
uniform vec2 viewportSize = vec2(1920.0, 1080.0);
uniform bool useInstanced;

void main()
{
    mat4 modelMatrix;
    
    if (useInstanced)
    {
        modelMatrix = mat4(
            instanceModel0,
            instanceModel1,
            instanceModel2,
            instanceModel3
        );
    }
    else
    {
        modelMatrix = model;
    }
    
    // Transform position to clip space
    vec4 clipPosition = vec4(aPosition, 1.0) * modelMatrix * view * projection;
    
    // Transform normal to view space
    vec3 viewNormal = normalize(vec3(vec4(aNormal, 0.0) * modelMatrix * view));
    
    // Calculate outline offset in view space
    vec3 outlineOffset = viewNormal * outlineThickness;
    
    // Transform offset to clip space (as direction vector, w=0)
    vec4 clipOffset = vec4(outlineOffset, 0.0) * projection;
    
    // Convert to NDC space for proper perspective compensation
    // In NDC space, coordinates range from -1 to 1
    // We need to scale the offset to maintain constant pixel thickness
    vec2 ndcOffset = clipOffset.xy / clipPosition.w;
    
    // Scale offset in NDC space to maintain constant screen-space size
    // The viewport height determines how many pixels correspond to NDC range [-1, 1]
    // NDC range [-1, 1] maps to viewportSize.y pixels, so 1 NDC unit = viewportSize.y / 2 pixels
    // To get constant pixel thickness, we scale by (outlineThickness / (viewportSize.y / 2))
    // Simplified: outlineThickness * 2.0 / viewportSize.y
    float ndcScale = outlineThickness * 6 / viewportSize.y;
    vec2 ndcScaledOffset = ndcOffset * ndcScale;
    
    // Convert back to clip space by multiplying by w
    // This compensates for perspective projection
    vec2 clipScaledOffset = ndcScaledOffset * clipPosition.w;
    
    // Apply the scaled offset
    clipPosition.xy += clipScaledOffset;
    
    gl_Position = clipPosition;
}
