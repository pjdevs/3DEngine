using gl.Rendering.Camera;

namespace gl.Rendering
{
    public abstract class RenderingTechnique
    {
        public Shader Shader { get; private set; }

        public RenderingTechnique(Shader shader)
        {
            Shader = shader;
        }

        public abstract bool CanRender(Model model);
        public abstract void Render(AbstractCamera camera, DirectionalLight dirLight, PointLight[] lights, Model model);
        public void ReloadShader()
        {
            Shader = Shader.Reload();
        }
    }
}