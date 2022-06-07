using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace gl.Rendering.Camera
{

    public class SphericalCamera : ICamera
    {
        private float _distance;
        private float _phi;
        private float _theta;
        private Vector3 _target;

        public Vector3 Position { get; private set; }
        public float AspectRatio { get; set; }
        public float Fov { get; set; }
        public float Near { get; set; }
        public float Far { get; set; }

        public Vector3 Target
        {
            get => _target;
            set
            {
                _target = value;
                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            _theta = MathHelper.Clamp(_theta, 0.0001f, MathHelper.PiOver2 - 0.0001f);
            _distance = MathHelper.Clamp(_distance, Near, Far);

            Position = _target + _distance * new Vector3(MathF.Cos(_phi) * MathF.Cos(_theta), MathF.Sin(_theta), MathF.Sin(_phi) * MathF.Cos(_theta));
        }

        public SphericalCamera(float aspectRatio)
        {
            _distance = 5f;
            _phi = 0f;
            _theta = MathHelper.PiOver6;

            Target = Vector3.Zero;
            AspectRatio = aspectRatio;
            Fov = MathHelper.DegreesToRadians(80f);
            Near = 0.01f;
            Far = 100f;
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Target, Vector3.UnitY);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(Fov, AspectRatio, Near, Far);
        }

        public void Update(KeyboardState keyboard, MouseState mouse, float dt)
        {
            if (mouse.IsButtonDown(MouseButton.Right))
            {
                _phi += mouse.Delta.X * dt;
                _theta += mouse.Delta.Y * dt;

            }

            _distance -= mouse.ScrollDelta.Y;

            UpdatePosition();
        }
    }
}