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
        private readonly Shader[] _shaders;
        private int _currentShader;
        private readonly AbstractCamera _camera;
        private readonly Model _sphere;
        private readonly Model _light;
        private readonly Light[] _lights;
        // private double _t;

        public GLWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            var uniforms = new string[]
            {
                "material.light"
            };

            _shaders = new Shader[]
            {
                new Shader("Shaders/phong.vert", "Shaders/phong.frag"),
                new Shader("Shaders/normal.vert", "Shaders/normal.frag")
            };
            _currentShader = 0;
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
            _sphere.Material.Shininess = 4;

            _lights = new Light[]
            {
                new Light()
                {
                    Position = new Vector3(0f, 3f, 1f),
                    Diffuse = Color4.LightYellow,
                    Constant = 0f,
                    Linear = 0f,
                    Quadratic = 0.1f,
                }
            };

            _light = new Model(MeshBuilder.BuildCube(0.1f));
            _light.Transform.Translation = _lights[0].Position;
            _light.Material.Ambient = Color4.White;
            _light.Material.Diffuse = Color4.White;
            _light.Material.Specular = Color4.White;
            _light.Material.Light = false;

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
            Renderer.Render(_shaders[_currentShader], _camera, _lights, _sphere);
            Renderer.Render(_shaders[0], _camera, _lights, _light);

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
                ++_currentShader;

                if (_currentShader >= _shaders.Length)
                    _currentShader = 0;
            }

            if (KeyboardState.IsKeyReleased(Keys.R))
            {
                for (var i = 0; i < _shaders.Length; ++i)
                {
                    _shaders[i] = _shaders[i].Reload();
                }
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
