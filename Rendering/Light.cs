using OpenTK.Mathematics;

namespace gl.Rendering
{
    public struct Light
    {
        public Vector3 Position;
        public Color4 Color;

        public Light()
        {
            Position = Vector3.One * 5f;
            Color = Color4.White;
        }
    }
}