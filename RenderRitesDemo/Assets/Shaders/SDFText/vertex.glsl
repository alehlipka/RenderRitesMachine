#version 460 core

layout (location = 0) in vec4 vertex; // <x, y, texX, texY>

out vec2 TexCoords;

uniform mat4 projection;
uniform vec2 position;
uniform vec2 scale;

void main()
{
    vec2 pos = position + vertex.xy * scale;
    gl_Position = projection * vec4(pos, 0.0, 1.0);
    TexCoords = vertex.zw;
}
