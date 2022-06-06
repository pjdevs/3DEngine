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
        private readonly AbstractCamera _camera;
        private readonly Model _sphere;
        private readonly Model _lightModel;
        private readonly DirectionalLight _dirLight;
        private readonly PointLight[] _lights;

        // private double _t;

        public GLWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _camera = new SphericalCamera(new Vector3(0f, 2f, 4f), Size.X / (float)Size.Y)
            {
                Pitch = -45f
            };

            var mesh = MeshBuilder.BuildPlane(5f, 5f); // MeshImporter.FromOBJ("Resources/cube.obj");

            _sphere = new Model(mesh);
            _sphere.Material.Ambient = Color4.Gray;
            _sphere.Material.Diffuse = Color4.White;
            _sphere.Material.Specular = Color4.White;
            _sphere.Material.Texture = Texture.LoadFromFile("Resources/rustediron/rustediron2_basecolor.png");
            _sphere.Material.Normal = Texture.LoadFromFile("Resources/rustediron/rustediron2_normal.png");
            _sphere.Material.Metallic = Texture.LoadFromFile("Resources/rustediron/rustediron2_metallic.png");
            _sphere.Material.Roughness = Texture.LoadFromFile("Resources/rustediron/rustediron2_roughness.png");
            _sphere.Material.Light = true;
            _sphere.Material.Shininess = 32;

            _dirLight = new DirectionalLight();

            _lights = new PointLight[]
            {
                new PointLight()
                {
                    Position = new Vector3(0f, 3f, 1f),
                    Diffuse = Color4.LightYellow,
                    Constant = 0f,
                    Linear = 0f,
                    Quadratic = 0.1f,
                }
            };

            _lightModel = new Model(MeshBuilder.BuildCube(0.1f));
            _lightModel.Transform.Translation = _lights[0].Position;
            _lightModel.Material.Ambient = Color4.White;
            _lightModel.Material.Diffuse = Color4.White;
            _lightModel.Material.Specular = Color4.White;
            _lightModel.Material.Light = false;

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
            Renderer.Render(_camera, _dirLight, _lights, _sphere);
            Renderer.Render(_camera, _dirLight, _lights, _lightModel);

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

            if (KeyboardState.IsKeyReleased(Keys.P))
            {
                _sphere.Transform.Rotation.X += MathHelper.PiOver2;
            }

            if (KeyboardState.IsKeyReleased(Keys.M))
            {
                Renderer.SwitchTechnique();
            }

            if (KeyboardState.IsKeyReleased(Keys.R))
            {
                Renderer.ReloadShaders();
            }

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
