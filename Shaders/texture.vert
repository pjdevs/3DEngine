#version 330

layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec2 vertexTexCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec2 texCoords;

void main(void)
{
    texCoords = vertexTexCoords;
    gl_Position = vec4(vertexPosition, 1.0) * model * view * projection;
}