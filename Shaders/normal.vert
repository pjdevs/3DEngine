#version 330

#define MAX_POINT_LIGHTS 10

struct Camera
{
    vec3 position;
};

struct Light
{
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};

layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec2 vertexTexCoords;
layout(location = 2) in vec3 vertexNormal;
layout(location = 3) in vec3 vertexTangent;

uniform int nbPointLights;
uniform Light pointLights[MAX_POINT_LIGHTS];
uniform Camera camera;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec2 texCoords;
out vec3 normal;
out vec3 tangent;
out vec3 worldPosition;
out vec3 viewPosition;
out vec3 lightPositions[MAX_POINT_LIGHTS];
out mat3 TBN;

void main()
{
    texCoords = vertexTexCoords;
    gl_Position = vec4(vertexPosition, 1.0) * model * view * projection;

    // Compute tangent space positions
    vec3 T = normalize(vec3(vec4(vertexTangent, 0.0) * model));
    vec3 N = normalize(vec3(vec4(vertexNormal, 0.0) * model));
    T = normalize(T - dot(T, N) * N);
    vec3 B = cross(N, T);

    TBN = mat3(T, B, N);
    
    for (int i = 0; i < nbPointLights; ++i)
        lightPositions[i] = pointLights[i].position * TBN;
    
    viewPosition = camera.position * TBN;
    worldPosition = vec3(vec4(vertexPosition, 1.0) * model) * TBN;
}