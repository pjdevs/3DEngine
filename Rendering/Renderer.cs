using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace gl.Rendering
{
    public static class Renderer
    {
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

        public static void Render(Shader shader, Camera camera, Light light, Model model)
        {
            shader.Use();

            shader.SetMatrix4("projection", camera.GetProjectionMatrix());
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("model", model.Transform.GetModelMatrix());
            shader.SetColor4("material.color", model.Material.Color);
            shader.SetInt("material.light", Convert.ToInt32(model.Material.Light));
            shader.SetFloat("material.ambient", model.Material.Ambient);
            shader.SetFloat("material.specular", model.Material.Specular);
            shader.SetInt("material.shininess", model.Material.Shininess);
            shader.SetColor4("light.color", light.Color);
            shader.SetVector3("light.position", light.Position);
            shader.SetVector3("camera.position", camera.Position);

            if (model.Material.Texture != null)
            {
                model.Material.Texture.Use(TextureUnit.Texture0);
                shader.SetInt("material.texture", 1);
            }

            model.Mesh.Draw();
        }

        public static void Viewport(int x, int y, int width, int height)
        {
            GL.Viewport(x, y, width, height);
        }
    }
}