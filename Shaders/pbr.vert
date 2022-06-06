#version 330

#define MAX_NUMBER_POINT_LIGHT 10

struct DirLight
{
    vec3 direction;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct PointLight
{
    vec3 position;

    float constant;
    float linear;
    float quadratic;
	
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoords;
layout(location = 2) in vec3 aNormal;
layout(location = 3) in vec3 aTangent;

uniform int nbPointLights;
uniform PointLight pointLights[MAX_NUMBER_POINT_LIGHT];
uniform DirLight dirLight;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 viewPos;

out VS_OUT {
    vec3 FragPos;
    vec3 ViewPos;
    vec2 TexCoords;
    vec3 LightPositions[MAX_NUMBER_POINT_LIGHT];
    vec3 DirLightDirection;
} vs_out;

void main()
{
    vs_out.TexCoords = aTexCoords;
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;

    // Compute tangent space positions
    vec3 T = normalize(vec3(vec4(aTangent, 0.0) * model));
    vec3 N = normalize(vec3(vec4(aNormal, 0.0) * model));
    T = normalize(T - dot(T, N) * N);
    vec3 B = cross(N, T);

    mat3 TBN = mat3(T, B, N);
    
    for (int i = 0; i < nbPointLights; ++i)
        vs_out.LightPositions[i] = pointLights[i].position * TBN;

    vs_out.DirLightDirection = dirLight.direction * TBN;
    vs_out.ViewPos = viewPos * TBN;
    vs_out.FragPos = vec3(vec4(aPosition, 1.0) * model) * TBN;
}