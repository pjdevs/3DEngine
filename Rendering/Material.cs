using OpenTK.Mathematics;

namespace gl.Rendering
{
    public class Material
    {
        public Color4 BaseColor;
        public bool Light;
        public Color4 Ambient;
        public Color4 Diffuse;
        public Color4 Specular;
        public int Shininess;

        public Texture? Texture;
        public Texture? Normal;
        public Texture? Metallic;
        public Texture? Roughness;
        public Texture? Height;

        public float HeightScale;
        public bool ParallaxHeight;

        public Material()
        {
            BaseColor = Color4.White;
            Light = false;
            Ambient = BaseColor;
            Diffuse = BaseColor;
            Specular = Color4.Gray;
            Shininess = 32;
            Texture = null;
            Normal = null;
            Metallic = null;
            Roughness = null;
            Height = null;
            HeightScale = 1f;
            ParallaxHeight = false;
        }
    }
}