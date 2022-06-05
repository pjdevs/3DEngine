#version 330

layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec2 vertexTexCoords;
layout(location = 2) in vec3 vertexNormal;
layout(location = 3) in vec3 vertexTangent;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec2 texCoords;
out vec3 normal;
out vec3 tangent;
out vec3 worldPosition;
out mat3 TBN;

void main(void)
{
    texCoords = vertexTexCoords;
    normal = vertexNormal * mat3(transpose(inverse(model)));
    worldPosition = vec3(vec4(vertexPosition, 1.0) * model);
    gl_Position = vec4(vertexPosition, 1.0) * model * view * projection;

    vec3 T = normalize(vec3(model * vec4(vertexTangent, 0.0)));
    vec3 N = normalize(vec3(model * vec4(vertexNormal, 0.0)));
    T = normalize(T - dot(T, N) * N);
    vec3 B = cross(N, T);

    TBN = mat3(T, B, N);
}