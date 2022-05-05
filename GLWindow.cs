using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using gl.Rendering;
using gl.Utils;

namespace gl
{
    public class GLWindow : GameWindow
    {
        private readonly Shader _shader;
        private readonly Camera _camera;
        private readonly Texture _texture;
        private readonly Model _cube;
        private readonly Light _light;
        private double _t;

        public GLWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _shader = new Shader("Shaders/texture.vert", "Shaders/texture.frag");
            _camera = new Camera(new Vector3(0f, 2f, 4f), Size.X / (float)Size.Y)
            {
                Pitch = -45f
            };
            _texture = Texture.LoadFromFile("Resources/container.png");

            var data = MeshImporter.FromOBJ("Resources/sphere.obj");

            _cube = new Model(new Mesh(data.Vertices, data.Indexes, MeshFlags.All));
            _cube.Material.Color = Color4.Red;
            // _cube.Material.Texture = _texture;
            _cube.Material.Light = true;
            _cube.Material.Shininess = 32;

            _light = new Light()
            {
                Color = Color4.LightYellow
            };

            _t = 0;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            Renderer.Setup();
            Renderer.ClearColor(Color4.Black);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            Renderer.Clear();
            Renderer.Render(_shader, _camera, _light, _cube);

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

            // _cube.Transform.Rotation.X += (float)e.Time * MathHelper.PiOver2;
            // _cube.Transform.Rotation.Y += (float)e.Time * 1.5f * MathHelper.PiOver2;

            // var s = (float)MathHelper.Sin(_t);
            // var t = (float)MathHelper.Sin(3f * _t);
            // _cube.Material.Color.R = s * s;
            // _cube.Material.Color.B = 1f - t * t;

            _t += e.Time;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            _camera.AspectRatio = e.Size.X / (float)e.Size.Y;

            Renderer.Viewport(0, 0, Size.X, Size.Y);
        }
    }
}
