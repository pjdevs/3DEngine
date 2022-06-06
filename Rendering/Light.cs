using OpenTK.Mathematics;

namespace gl.Rendering
{
    public struct PointLight
    {
        public Vector3 Position;
        public Color4 Ambient;
        public Color4 Diffuse;
        public Color4 Specular;

        public float Constant;
        public float Linear;
        public float Quadratic;

        public PointLight()
        {
            Position = Vector3.One * 5f;
            Ambient = new Color4(0.2f, 0.2f, 0.2f, 1f);
            Diffuse = Color4.LightYellow;
            Specular = Color4.White;

            Constant = 0f;
            Linear = 0f;
            Quadratic = 0.1f;
        }
    }

    public struct DirectionalLight
    {
        public Vector3 Direction;
        public Color4 Ambient;
        public Color4 Diffuse;
        public Color4 Specular;

        public DirectionalLight()
        {
            Direction = -Vector3.One;
            Ambient = new Color4(0.2f, 0.2f, 0.2f, 1f);
            Diffuse = Color4.LightYellow;
            Specular = Color4.White;
        }
    }
}