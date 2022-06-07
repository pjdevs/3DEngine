using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace gl.Rendering.Camera
{

    public interface ICamera
    {
        public Vector3 Position { get; }
        public float AspectRatio { get; set; }
        public float Fov { get; set; }
        public float Near { get; set; }
        public float Far { get; set; }

        public Matrix4 GetViewMatrix();
        public Matrix4 GetProjectionMatrix();
        public void Update(KeyboardState keyboard, MouseState mouse, float dt);
    }
}