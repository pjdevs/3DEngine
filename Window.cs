using gl.Rendering;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace gl
{
    public class Window : GameWindow
    {
        private readonly Shader _shader;
        private readonly Camera _camera;
        private readonly Texture _texture;
        private readonly Model _plane;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _shader = new Shader("Shaders/texture.vert", "Shaders/texture.frag");
            _camera = new Camera(3f * Vector3.UnitZ, Size.X / (float)Size.Y);
            _texture = Texture.LoadFromFile("Resources/container.png");

            _plane = new Model(MeshBuilder.BuildCube(1));
            _plane.Material.Color = Color4.White;
            _plane.Material.Texture = _texture;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            Renderer.Setup();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            Renderer.Clear();
            Renderer.Draw(_shader, _camera, _plane);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            _plane.Transform.Rotation.X += (float)e.Time * MathHelper.PiOver2;
            _plane.Transform.Rotation.Y += (float)e.Time * 1.5f * MathHelper.PiOver2;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            _camera.AspectRatio = e.Size.X / (float)e.Size.Y;

            Renderer.Viewport(0, 0, Size.X, Size.Y);
        }
    }
}
