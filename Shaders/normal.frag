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

uniform Material material;
uniform int nbPointLights;
uniform Light pointLights[MAX_POINT_LIGHTS];

in vec2 texCoords;
in vec3 viewPosition;
in vec3 worldPosition;
in vec3 lightPositions[MAX_POINT_LIGHTS];
in mat3 TBN;

out vec4 fragColor;

vec3 ComputePointLight(Light light, vec3 lightPosition, vec3 norm, vec3 viewDirection, vec3 materialAmbient, vec3 materialDiffuse, vec3 materialSpecular)
{
    // Directions
    vec3 lightDirection = normalize(lightPosition - worldPosition);

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
    float d = length(lightPosition - worldPosition);
    float attenuation = 1.0 / (light.constant + light.linear * d + light.quadratic * (d * d));

    return (ambientLight + diffuseLight + specularLight) * attenuation;
}

void main()
{
    // Final material props
    vec3 materialAmbient = material.ambient;
    vec3 materialDiffuse = material.diffuse;;
    vec3 materialSpecular = material.specular;

    if (material.hasTexture)
    {
        materialAmbient *= texture(material.texture, texCoords).rgb;
        materialDiffuse *= texture(material.texture, texCoords).rgb;
        materialSpecular *= texture(material.texture, texCoords).rgb;
    }
    // Directions
    vec3 norm = texture(material.normal, texCoords).rgb;
    norm = normalize(norm * 2.0 - 1.0);
    vec3 viewDirection = normalize(viewPosition - worldPosition);

    // Lights
    vec3 color = vec3(0.0);

    if (material.light)
    {
        for (int i = 0; i < nbPointLights; ++i)
            color += ComputePointLight(pointLights[i], lightPositions[i], norm, viewDirection, materialAmbient, materialDiffuse, materialSpecular);
    }
    else
        color = materialDiffuse;

    fragColor = vec4(color, 1.0);
}