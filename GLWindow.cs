using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using gl.Rendering;
using gl.Rendering.Camera;
using gl.Rendering.Utils;

namespace gl
{
    public class GLWindow : GameWindow
    {
        private readonly Shader _shader;
        private readonly AbstractCamera _camera;
        private readonly Model _sphere;
        private readonly Light[] _lights;
        // private double _t;

        public GLWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _shader = new Shader("Shaders/normal.vert", "Shaders/normal.frag");
            _camera = new SphericalCamera(new Vector3(0f, 2f, 4f), Size.X / (float)Size.Y)
            {
                Pitch = -45f
            };

            var mesh = MeshBuilder.BuildPlane(5f, 5f); // MeshImporter.FromOBJ("Resources/cube.obj");

            _sphere = new Model(mesh);
            _sphere.Material.Ambient = Color4.Gray;
            _sphere.Material.Diffuse = Color4.White;
            _sphere.Material.Specular = Color4.White;
            _sphere.Material.Texture = Texture.LoadFromFile("Resources/brickwall/brickwall.jpg");
            _sphere.Material.Normal = Texture.LoadFromFile("Resources/brickwall/brickwall_normal.jpg");
            _sphere.Material.Light = true;
            _sphere.Material.Shininess = 32;

            _lights = new Light[]
            {
                new Light()
                {
                    Position = _camera.Position,
                    Diffuse = Color4.LightYellow
                }
            };

            // _t = 0;
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
            Renderer.Render(_shader, _camera, _lights, _sphere);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused)
            {
                return;
            }

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (KeyboardState.IsKeyReleased(Keys.R))
            {
                _sphere.Transform.Rotation.X += MathHelper.PiOver2;
            }

            // _sphere.Transform.Rotation.Y += (float)e.Time * 1.5f * MathHelper.PiOver2;

            // var s = (float)MathHelper.Sin(_t);
            // var t = (float)MathHelper.Sin(3f * _t);
            // _cube.Material.Color.R = s * s;
            // _cube.Material.Color.B = 1f - t * t;

            // _t += e.Time;

            _camera.Update(KeyboardState, MouseState, (float)e.Time);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            _camera.AspectRatio = e.Size.X / (float)e.Size.Y;

            Renderer.Viewport(0, 0, Size.X, Size.Y);
        }
    }
}
