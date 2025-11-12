#version 460 core

layout (location = 0) in vec3 aPosition;

layout (location = 3) in vec4 instanceModel0;
layout (location = 4) in vec4 instanceModel1;
layout (location = 5) in vec4 instanceModel2;
layout (location = 6) in vec4 instanceModel3;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
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
    
    gl_Position = vec4(aPosition, 1.0) * modelMatrix * view * projection;
}
