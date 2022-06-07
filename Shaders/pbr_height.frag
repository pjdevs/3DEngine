#version 330 core

#define PI 3.14159265359
#define MAX_NUMBER_POINT_LIGHT 10

struct Material
{
    sampler2D albedoMap;
    sampler2D normalMap;
    sampler2D metallicMap;
    sampler2D roughnessMap;
    sampler2D heightMap;
    // sampler2D aoMap;

    float heightScale;
    bool parallaxHeight;
};

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

struct LightingInfo
{
    // Light
    vec3 direction;
    float attenuation;
    vec3 color;

    // Material
    vec3 albedo;
    float metallic;
    float roughness;
    float ao;
};

in VS_OUT {
    vec3 FragPos;
    vec3 ViewPos;
    vec2 TexCoords;
    vec3 LightPositions[MAX_NUMBER_POINT_LIGHT];
    vec3 DirLightDirection;
} fs_in;

uniform Material material;
uniform DirLight dirLight;
uniform int nbPointLightInScene;
uniform PointLight pointLights[MAX_NUMBER_POINT_LIGHT];

out vec4 FragColor;

// FO is the surface reflection at zero incidence or how much the surface reflects if looking directly at the surface.
// The F0 varies per material and is tinted on metals as we find in large material databases.
// In the PBR metallic workflow we make the simplifying assumption that most dielectric surfaces look visually correct
// with a constant F0 of 0.04, while we do specify F0 for metallic surfaces as then given by the albedo value
vec3 FresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}

float DistributionGGX(vec3 N, vec3 H, float roughness)
{
    float a      = roughness*roughness;
    float a2     = a*a;
    float NdotH  = max(dot(N, H), 0.0);
    float NdotH2 = NdotH*NdotH;
	
    float num   = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;
	
    return num / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r*r) / 8.0;

    float num   = NdotV;
    float denom = NdotV * (1.0 - k) + k;
	
    return num / denom;
}
float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2  = GeometrySchlickGGX(NdotV, roughness);
    float ggx1  = GeometrySchlickGGX(NdotL, roughness);
	
    return ggx1 * ggx2;
}

float ComputeAttenuation(PointLight light, vec3 lightPos)
{
    float distance = length(lightPos - fs_in.FragPos);
    return 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance)); 
}

vec3 ComputeLightContribution(LightingInfo info, vec3 viewDir, vec3 normal)
{
    // Directions
    vec3 halfwayDir = normalize(viewDir + info.direction);

    // Compute attenuation and radiance
    vec3 radiance = info.color * info.attenuation;

    // Compute reflection at zero incidence and real reflection combining albedo and metallic
    vec3 F0 = vec3(0.04); 
    F0 = mix(F0, info.albedo, info.metallic);
    vec3 F = FresnelSchlick(max(dot(halfwayDir, viewDir), 0.0), F0);

    // Compute Normal Distribution Function
    float NDF = DistributionGGX(normal, halfwayDir, info.roughness);       
    float G = GeometrySmith(normal, viewDir, info.direction, info.roughness);

    // Compute Cook-Torence BRDF (Bidirectional Reflective Distribution Function)
    vec3 numerator = NDF * G * F;
    float denominator = 4.0 * max(dot(normal, viewDir), 0.0) * max(dot(normal, info.direction), 0.0)  + 0.0001; // prevent dividing by zero
    vec3 specular = numerator / denominator;

    // Contribution in the reflectance equation.
    // kS represents the energy of light that gets reflected,
    // the remaining ratio of light energy is the light that gets refracted which we store as kD.
    // kD is zero if metallic cause it don't refract light.
    vec3 kS = F;
    vec3 kD = vec3(1.0) - kS;
    kD *= 1.0 - info.metallic;

    // Add outgoing radiance to the final color
    float NdotL = max(dot(normal, info.direction), 0.0);        
    return (kD * info.albedo / PI + specular) * radiance * NdotL;
}

vec2 ParallaxMapping(vec2 texCoords, vec3 viewDir)
{ 
    // Basic version

    // float height = texture(material.heightMap, texCoords).r;
    // vec2 p = viewDir.xy / viewDir.z * (height * material.heightScale);
    // return texCoords - p;

    // Steep Parallax Mapping

    // number of depth layers
    const float numLayers = 10;
    // calculate the size of each layer
    float layerDepth = 1.0 / numLayers;
    // depth of current layer
    float currentLayerDepth = 0.0;
    // the amount to shift the texture coordinates per layer (from vector P)
    vec2 P = viewDir.xy * material.heightScale; 
    vec2 deltaTexCoords = P / numLayers;

    // get initial values
    vec2  currentTexCoords = texCoords;
    float currentDepthMapValue = texture(material.heightMap, texCoords).r; 
    
    while(currentLayerDepth < currentDepthMapValue)
    {
        // shift texture coordinates along direction of P
        currentTexCoords -= deltaTexCoords;
        // get depthmap value at current texture coordinates
        currentDepthMapValue = texture(material.heightMap, texCoords).r;
        // get depth of next layer
        currentLayerDepth += layerDepth;  
    }

    // get texture coordinates before collision (reverse operations)
    vec2 prevTexCoords = currentTexCoords + deltaTexCoords;

    // get depth after and before collision for linear interpolation
    float afterDepth  = currentDepthMapValue - currentLayerDepth;
    float beforeDepth = texture(material.heightMap, prevTexCoords).r - currentLayerDepth + layerDepth;
    
    // interpolation of texture coordinates
    float weight = afterDepth / (afterDepth - beforeDepth);
    vec2 finalTexCoords = prevTexCoords * weight + currentTexCoords * (1.0 - weight);

    return finalTexCoords; 
} 

void main()
{
    // Pre compute
    vec3 viewDir = normalize(fs_in.ViewPos - fs_in.FragPos);
    vec2 texCoords = fs_in.TexCoords;
    
    if (material.parallaxHeight)
    {
        texCoords = ParallaxMapping(fs_in.TexCoords, viewDir);
    }

    vec3 normal = texture(material.normalMap, texCoords).rgb;
    normal = normalize(normal * 2.0 - 1.0);

    // Resulting color
    vec3 color = vec3(0.0);

    // Load material values from maps
    LightingInfo info;
    info.albedo = pow(texture(material.albedoMap, texCoords).rgb, vec3(2.2));
    info.metallic = texture(material.metallicMap, texCoords).r;
    info.roughness = texture(material.roughnessMap, texCoords).r;
    info.ao = 1.0; // texture(material.aoMap, texCoords).r;

    // Compute directional light
    info.direction = normalize(-fs_in.DirLightDirection);
    info.attenuation = 1.0;
    info.color = dirLight.diffuse;

    color += ComputeLightContribution(info, viewDir, normal);

    // Compute point lights
    for (int i = 0; i < nbPointLightInScene; ++i)
    {
        info.direction = normalize(fs_in.LightPositions[i] - fs_in.FragPos);
        info.attenuation = ComputeAttenuation(pointLights[i], fs_in.LightPositions[i]);
        info.color = pointLights[i].diffuse;

        color += ComputeLightContribution(info, viewDir, normal);
    }

    vec3 ambient = vec3(0.03) * info.albedo * info.ao;
    color += ambient;

    // Gamma correction / HDR
    color = color / (color + vec3(1.0));
    color = pow(color, vec3(1.0/2.2));

    FragColor = vec4(color, 1.0);
}