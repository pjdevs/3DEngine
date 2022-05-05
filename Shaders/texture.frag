#version 330

struct Material
{
    vec3 color;
    bool texture;
    bool light;
    float ambient;
    float specular;
    int shininess;
};

struct Light
{
    vec3 color;
    vec3 position;
};

struct Camera
{
    vec3 position;
};

uniform Material material;
uniform Light light;
uniform Camera camera;

uniform sampler2D texture0;

in vec2 texCoords;
in vec3 normal;
in vec3 worldPosition;

out vec4 fragColor;

void main(void)
{
    fragColor = vec4(material.color, 1.0);

    if (material.texture)
    {   
        fragColor *= texture(texture0, texCoords);
    }

    if (material.light)
    {
        vec3 norm = normalize(normal);
        vec3 ambientLight = material.ambient * light.color;
        
        vec3 lightDirection = normalize(light.position - worldPosition);
        float diffuseStrength = max(dot(norm, lightDirection), 0.0);
        vec3 diffuseLight = diffuseStrength * light.color;

        vec3 viewDirection = normalize(camera.position - worldPosition);
        vec3 reflectionDirection = reflect(-viewDirection, norm);
        float specularStrength = pow(max(dot(viewDirection, reflectionDirection), 0.0), material.shininess);
        vec3 specularLight = material.specular * specularStrength * light.color; 


        fragColor *= vec4(ambientLight + diffuseLight + specularLight, 1.0);
    }
}