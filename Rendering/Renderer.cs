using OpenTK.Graphics.OpenGL4;

namespace gl.Rendering
{
    public static class Renderer
    {
        public static void Setup()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        }

        public static void Clear()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public static void Draw(Shader shader, Camera camera, Model model)
        {
            shader.Use();

            shader.SetMatrix4("projection", camera.GetProjectionMatrix());
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("model", model.Transform.GetModelMatrix());
            shader.SetColor4("material.color", model.Material.Color);

            model.Material.Texture?.Use(TextureUnit.Texture0);
            model.Mesh.Draw();
        }

        public static void Viewport(int x, int y, int width, int height)
        {
            GL.Viewport(x, y, width, height);
        }
    }
}