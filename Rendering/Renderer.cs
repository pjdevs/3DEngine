using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using gl.Rendering.Camera;

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

        public static void Render(Shader shader, AbstractCamera camera, Light[] lights, Model model)
        {
            shader.Use();

            shader.SetMatrix4("projection", camera.GetProjectionMatrix());
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("model", model.Transform.GetModelMatrix());

            shader.SetColor4("material.baseColor", model.Material.BaseColor);
            shader.SetInt("material.light", Convert.ToInt32(model.Material.Light));
            shader.SetColor4("material.ambient", model.Material.Ambient);
            shader.SetColor4("material.diffuse", model.Material.Diffuse);
            shader.SetColor4("material.specular", model.Material.Specular);
            shader.SetInt("material.shininess", model.Material.Shininess);

            for (var i = 0; i < lights.Length; ++i)
            {
                shader.SetVector3($"pointLights[{i}].position", lights[i].Position);
                shader.SetColor4($"pointLights[{i}].ambient", lights[i].Ambient);
                shader.SetColor4($"pointLights[{i}].diffuse", lights[i].Diffuse);
                shader.SetColor4($"pointLights[{i}].specular", lights[i].Specular);
                shader.SetFloat($"pointLights[{i}].constant", lights[i].Constant);
                shader.SetFloat($"pointLights[{i}].linear", lights[i].Linear);
                shader.SetFloat($"pointLights[{i}].quadratic", lights[i].Quadratic);
            }
            shader.SetInt("nbPointLights", lights.Length);

            shader.SetVector3("camera.position", camera.Position);

            if (model.Material.Texture != null)
            {
                model.Material.Texture.Use(TextureUnit.Texture0);
                shader.SetInt("material.hasTexture", 1);
            }
            else
            {
                shader.SetInt("material.hasTexture", 0);
            }

            if (model.Material.Normal != null)
            {
                model.Material.Normal.Use(TextureUnit.Texture1);
            }

            shader.SetInt("material.texture", 0);
            try
            {
                shader.SetInt("material.normal", 1);
            }
            catch (Exception _)
            {
            }

            model.Mesh.Draw();
        }

        public static void Viewport(int x, int y, int width, int height)
        {
            GL.Viewport(x, y, width, height);
        }
    }
}