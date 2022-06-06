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
        private readonly Model _model;
        private readonly Model _lightModel;
        private readonly DirectionalLight _dirLight;
        private readonly PointLight[] _lights;
        private readonly Material[] _materials;
        private int _currentMaterial;

        // private double _t;

        public GLWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _camera = new SphericalCamera(new Vector3(0f, 2f, 4f), Size.X / (float)Size.Y)
            {
                Pitch = -45f
            };


            _materials = new Material[]
            {
                new Material()
                {
                    Ambient = Color4.Gray,
                    Diffuse = Color4.White,
                    Specular = Color4.White,
                    Light = true,
                    Shininess = 32,
                    Texture = Texture.LoadFromFile("Resources/caverdeposit/cavern-deposits_albedo.png"),
                    Normal = Texture.LoadFromFile("Resources/caverdeposit/cavern-deposits_normal.png"),
                    Metallic = Texture.LoadFromFile("Resources/caverdeposit/cavern-deposits_metallic.png"),
                    Roughness = Texture.LoadFromFile("Resources/caverdeposit/cavern-deposits_roughness.png")
                },
                new Material()
                {
                    Ambient = Color4.Gray,
                    Diffuse = Color4.White,
                    Specular = Color4.White,
                    Light = true,
                    Shininess = 32,
                    Texture = Texture.LoadFromFile("Resources/rustediron/rustediron2_basecolor.png"),
                    Normal = Texture.LoadFromFile("Resources/rustediron/rustediron2_normal.png"),
                    Metallic = Texture.LoadFromFile("Resources/rustediron/rustediron2_metallic.png"),
                    Roughness = Texture.LoadFromFile("Resources/rustediron/rustediron2_roughness.png")
                },
                new Material()
                {
                    Ambient = Color4.Gray,
                    Diffuse = Color4.White,
                    Specular = Color4.White,
                    Light = true,
                    Shininess = 32,
                    Texture = Texture.LoadFromFile("Resources/brickwall/brickwall.jpg"),
                    Normal = Texture.LoadFromFile("Resources/brickwall/brickwall_normal.jpg")
                }
            };
            _currentMaterial = 0;

            _dirLight = new DirectionalLight();
            _lights = new PointLight[]
            {
                new PointLight()
                {
                    Position = new Vector3(0f, 3f, 1f),
                    Constant = 0f,
                    Linear = 0f,
                    Quadratic = 0.1f,
                }
            };

            _model = new Model(MeshBuilder.BuildPlane(5f, 5f))
            {
                Material = _materials[_currentMaterial]
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
            Renderer.Render(_camera, _dirLight, _lights, _model);
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
                _model.Transform.Rotation.X += MathHelper.PiOver2;
            }
            if (KeyboardState.IsKeyReleased(Keys.M))
            {
                Renderer.SwitchTechnique();
            }
            if (KeyboardState.IsKeyReleased(Keys.R))
            {
                Renderer.ReloadShaders();
            }
            if (KeyboardState.IsKeyReleased(Keys.V))
            {
                _currentMaterial++;

                if (_currentMaterial >= _materials.Length)
                    _currentMaterial = 0;

                _model.Material = _materials[_currentMaterial];
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
