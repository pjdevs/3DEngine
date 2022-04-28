using OpenTK.Mathematics;

namespace gl.Rendering
{
    public struct Transform
    {
        public Vector3 Translation;
        public Vector3 Rotation;
        public Vector3 Scale;

        public Transform()
        {
            Translation = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = Vector3.One;
        }

        public Matrix4 GetModelMatrix()
        {
            return Matrix4.CreateScale(Scale)
                * Matrix4.CreateTranslation(Translation)
                * Matrix4.CreateFromAxisAngle(Vector3.UnitX, Rotation.X)
                * Matrix4.CreateFromAxisAngle(Vector3.UnitY, Rotation.Y)
                * Matrix4.CreateFromAxisAngle(Vector3.UnitZ, Rotation.Z);
        }
    }
}