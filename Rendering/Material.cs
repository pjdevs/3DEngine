using OpenTK.Mathematics;

namespace gl.Rendering
{
    public struct Material
    {
        public Color4 BaseColor;
        public Texture? Texture;
        public Texture? Normal;
        public bool Light;
        public Color4 Ambient;
        public Color4 Diffuse;
        public Color4 Specular;
        public int Shininess;

        public Material()
        {
            BaseColor = Color4.White;
            Texture = null;
            Normal = null;
            Light = false;
            Ambient = BaseColor;
            Diffuse = BaseColor;
            Specular = Color4.Gray;
            Shininess = 32;
        }
    }
}