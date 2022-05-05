using OpenTK.Mathematics;

namespace gl.Rendering
{
    public struct Material
    {
        public Color4 Color;
        public Texture? Texture;
        public bool Light;
        public float Ambient;
        public float Specular;
        public int Shininess;

        public Material()
        {
            Color = Color4.White;
            Texture = null;
            Light = false;
            Ambient = 0.1f;
            Specular = 0.5f;
            Shininess = 32;
        }
    }
}