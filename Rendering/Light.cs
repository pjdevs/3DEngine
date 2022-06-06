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
            Diffuse = new Color4(0.5f, 0.5f, 0.5f, 1f);
            Specular = Color4.White;

            Constant = 1f;
            Linear = 0f;
            Quadratic = 0f;
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
            Direction = -Vector3.One * 5f;
            Ambient = new Color4(0.2f, 0.2f, 0.2f, 1f);
            Diffuse = new Color4(0.5f, 0.5f, 0.5f, 1f);
            Specular = Color4.White;
        }
    }
}