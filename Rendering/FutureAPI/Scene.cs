using gl.Rendering;
using OpenTK.Mathematics;

namespace gl
{
    public class Scene : IDisposable
    {
        private readonly Dictionary<string, IDisposable> _resources;
        private readonly Shader _defaultShader;
        private Vector2i _size;
        private readonly Node _root;

        public Camera CurrentCamera { get; set; }
        public Shader CurrentShader { get; set; }

        protected void Draw(Model model)
        {
            // Renderer.Render(CurrentShader, CurrentCamera, model);
        }

        public Scene(Vector2i size, Node root)
        {
            _resources = new Dictionary<string, IDisposable>();
            _size = size;
            _root = root;
            _root.Scene = this;
            _defaultShader = new Shader("Shaders/texture.vert", "Shaders/texture.frag");

            CurrentCamera = new Camera(Vector3.Zero, _size.X / (float)_size.Y);
            CurrentShader = _defaultShader;
        }

        public T AddResource<T>(string name, T resource) where T : IDisposable
        {
            _resources.Add(name, resource);

            return resource;
        }

        public T GetResource<T>(string name) where T : IDisposable
        {
            return (T)_resources[name];
        }

        public virtual void Load()
        {
            _root.Load();
        }

        public void Render()
        {
            _root.Render();
        }

        public void Update(double deltaTime)
        {
            _root.Update(deltaTime);
        }

        public void Resize(Vector2i size)
        {
            _size = size;
            CurrentCamera.AspectRatio = _size.X / (float)_size.Y;

            Renderer.Viewport(0, 0, _size.X, _size.Y);
        }

        public void Dispose()
        {
            _defaultShader.Dispose();
            _root.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
