#version 330

struct Material
{
    vec3 color;
};

uniform sampler2D texture0;
uniform Material material;

in vec2 texCoords;

out vec4 frag_color;

void main(void)
{
    frag_color = vec4(material.color, 1.0) * texture(texture0, texCoords);
    // frag_color = texture(texture0, texCoords) * vec4(material.color, 1.0);
}