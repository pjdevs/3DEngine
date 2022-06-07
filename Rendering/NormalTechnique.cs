using gl.Rendering.Camera;
using OpenTK.Graphics.OpenGL4;

namespace gl.Rendering
{
    public class NormalTechnique : RenderingTechnique
    {
        public NormalTechnique()
            : base(new Shader("Shaders/normal.vert", "Shaders/normal.frag"))
        {
        }

        public override bool CanRender(Model model)
        {
            return model.Material.Normal != null;
        }

        public override void Render(AbstractCamera camera, DirectionalLight dirLight, PointLight[] lights, Model model)
        {
            Shader.Use();

            Shader.SetMatrix4("projection", camera.GetProjectionMatrix());
            Shader.SetMatrix4("view", camera.GetViewMatrix());
            Shader.SetMatrix4("model", model.Transform.GetModelMatrix());

            // Shader.SetColor4("material.baseColor", model.Material.BaseColor);
            Shader.SetInt("material.light", Convert.ToInt32(model.Material.Light));
            Shader.SetColor4("material.ambient", model.Material.Ambient);
            Shader.SetColor4("material.diffuse", model.Material.Diffuse);
            Shader.SetColor4("material.specular", model.Material.Specular);
            Shader.SetInt("material.shininess", model.Material.Shininess);

            for (var i = 0; i < lights.Length; ++i)
            {
                Shader.SetVector3($"pointLights[{i}].position", lights[i].Position);
                Shader.SetColor4($"pointLights[{i}].ambient", lights[i].Ambient);
                Shader.SetColor4($"pointLights[{i}].diffuse", lights[i].Diffuse);
                Shader.SetColor4($"pointLights[{i}].specular", lights[i].Specular);
                Shader.SetFloat($"pointLights[{i}].constant", lights[i].Constant);
                Shader.SetFloat($"pointLights[{i}].linear", lights[i].Linear);
                Shader.SetFloat($"pointLights[{i}].quadratic", lights[i].Quadratic);
            }
            Shader.SetInt("nbPointLights", lights.Length);

            Shader.SetVector3("camera.position", camera.Position);

            if (model.Material.Texture != null)
            {
                model.Material.Texture.Use(TextureUnit.Texture0);
                Shader.SetInt("material.hasTexture", 1);
            }
            else
            {
                Shader.SetInt("material.hasTexture", 0);
            }

            model.Material.Normal?.Use(TextureUnit.Texture1);

            Shader.SetInt("material.texture", 0);
            Shader.SetInt("material.normal", 1);

            model.Mesh.Draw();
        }
    }
}