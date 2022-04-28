using OpenTK.Mathematics;

namespace gl.Rendering
{
    public struct Material
    {
        public Color4 Color;
        public Texture? Texture;

        public Material()
        {
            Color = Color4.White;
            Texture = null;
        }
    }
}