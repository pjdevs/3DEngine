#version 330

#define MAX_POINT_LIGHTS 10

struct Material
{
    vec3 baseColor;
    bool hasTexture;
    bool light;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    int shininess;

    sampler2D texture;
    sampler2D normal;
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

struct Camera
{
    vec3 position;
};

uniform Material material;
uniform Camera camera;
uniform int nbPointLights;
uniform Light pointLights[MAX_POINT_LIGHTS];

in vec2 texCoords;
in vec3 normal;
in vec3 worldPosition;
in mat3 TBN;

out vec4 fragColor;

vec3 ComputePointLight(Light light, vec3 norm, vec3 viewDirection, vec3 materialAmbient, vec3 materialDiffuse, vec3 materialSpecular)
{
    // Directions
    vec3 lightDirection = normalize(light.position - worldPosition);

    // Ambient
    vec3 ambientLight = materialAmbient * light.ambient;
    
    // Diffuse
    float diffuseStrength = max(dot(norm, lightDirection), 0.0);
    vec3 diffuseLight = diffuseStrength * materialDiffuse * light.diffuse;

    // Specular
    vec3 reflectionDirection = reflect(-lightDirection, norm);
    float specularStrength = pow(max(dot(viewDirection, reflectionDirection), 0.0), material.shininess);
    vec3 specularLight = materialSpecular * specularStrength * light.specular; 

    // Attenuation
    float d = length(light.position - worldPosition);
    float attenuation = 1.0 / (light.constant + light.linear * d + light.quadratic * (d * d));

    return (ambientLight + diffuseLight + specularLight) * attenuation;
}

void main(void)
{
    // Final material props
    vec3 materialAmbient = material.ambient * texture(material.texture, texCoords).rgb;
    vec3 materialDiffuse = material.diffuse * texture(material.texture, texCoords).rgb;
    vec3 materialSpecular = material.specular * texture(material.texture, texCoords).rgb;

    // Directions
    vec3 norm = texture(material.normal, texCoords).rgb;
    norm = norm * 2.0 - 1.0;
    norm = normalize(TBN * norm);
    vec3 viewDirection = normalize(camera.position - worldPosition);

    // Lights
    vec3 color = vec3(0.0);

    if (material.light)
    {
        for (int i = 0; i < nbPointLights; ++i)
            color += ComputePointLight(pointLights[i], norm, viewDirection, materialAmbient, materialDiffuse, materialSpecular);
    }
    else
        color = materialDiffuse;

    fragColor = vec4(color, 1.0);
}