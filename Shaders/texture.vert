#version 330

layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec2 vertexTexCoords;
layout(location = 2) in vec3 vertexNormal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec2 texCoords;
out vec3 normal;
out vec3 worldPosition;

void main(void)
{
    texCoords = vertexTexCoords;
    normal = vertexNormal * mat3(transpose(inverse(model)));
    worldPosition = vec3(model * vec4(vertexPosition, 1.0));
    gl_Position = vec4(vertexPosition, 1.0) * model * view * projection;
}