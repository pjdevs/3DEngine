using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace gl.Rendering.Camera
{
    public class FreeCamera : StaticCamera
    {
        public float Sensivity { get; set; }
        public float Speed { get; set; }

        public FreeCamera(Vector3 position, float aspectRatio)
            : base(position, aspectRatio)
        {
            Sensivity = 10f;
            Speed = 10f;
        }

        public override void Update(KeyboardState keyboard, MouseState mouse, float dt)
        {
            if (keyboard.IsKeyDown(Keys.W))
            {
                Position += Front * Speed * dt;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                Position -= Right * Speed * dt;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                Position -= Front * Speed * dt;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                Position += Right * Speed * dt;
            }
            if (keyboard.IsKeyDown(Keys.Space))
            {
                Position += Vector3.UnitY * Speed * dt;
            }
            if (keyboard.IsKeyDown(Keys.LeftShift))
            {
                Position -= Vector3.UnitY * Speed * dt;
            }


            if (mouse.IsButtonDown(MouseButton.Right))
            {
                Pitch -= mouse.Delta.Y * dt * Sensivity;
                Yaw += mouse.Delta.X * dt * Sensivity;
            }
        }
    }
}