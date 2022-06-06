using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using gl.Rendering.Camera;

namespace gl.Rendering
{
    public static class Renderer
    {
        private static readonly Dictionary<string, RenderingTechnique> s_techniques = new()
        {
            { "PBR", new PBRTechnique() },
            { "Normal Map", new NormalTechnique() },
            { "Phong", new PhongTechnique() }
        };

        private static int s_technique = 0;

        private static RenderingTechnique? GetBestTechnique(Model model)
        {
            RenderingTechnique? bestTechnique = null;

            foreach (var technique in s_techniques.Values)
            {
                if (technique.CanRender(model))
                {
                    bestTechnique = technique;
                    break;
                }
            }

            return bestTechnique;
        }

        public static IEnumerable<string> GetTechniques()
        {
            return s_techniques.Keys.AsEnumerable();
        }

        public static void SwitchTechnique()
        {
            s_technique++;

            if (s_technique >= s_techniques.Values.Count)
                s_technique = 0;
        }

        public static void ClearColor(Color4 color)
        {
            GL.ClearColor(color);
        }

        public static void Setup()
        {
            GL.Enable(EnableCap.DepthTest);
        }

        public static void Clear()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public static void ReloadShaders()
        {
            foreach (var technique in s_techniques.Values)
            {
                technique.ReloadShader();
            }
        }

        public static void Render(AbstractCamera camera, DirectionalLight dirLight, PointLight[] lights, Model model)
        {
            var technique = s_techniques.Values.ElementAt(s_technique);

            if (!technique.CanRender(model))
                technique = GetBestTechnique(model);

            technique?.Render(camera, dirLight, lights, model);
        }

        public static void Viewport(int x, int y, int width, int height)
        {
            GL.Viewport(x, y, width, height);
        }
    }
}