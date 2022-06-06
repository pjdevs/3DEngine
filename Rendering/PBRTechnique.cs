using gl.Rendering.Camera;
using OpenTK.Graphics.OpenGL4;

namespace gl.Rendering
{
    public class PBRTechnique : RenderingTechnique
    {
        public PBRTechnique()
            : base(new Shader("Shaders/pbr.vert", "Shaders/pbr.frag"))
        {
        }

        public override bool CanRender(Model model)
        {
            return model.Material.Texture != null
                && model.Material.Normal != null
                && model.Material.Metallic != null
                && model.Material.Roughness != null;
        }

        public override void Render(AbstractCamera camera, DirectionalLight dirLight, PointLight[] lights, Model model)
        {
            Shader.Use();

            Shader.SetMatrix4("projection", camera.GetProjectionMatrix());
            Shader.SetMatrix4("view", camera.GetViewMatrix());
            Shader.SetMatrix4("model", model.Transform.GetModelMatrix());
            Shader.SetVector3("viewPos", camera.Position);

            model.Material.Texture?.Use(TextureUnit.Texture0);
            model.Material.Normal?.Use(TextureUnit.Texture1);
            model.Material.Metallic?.Use(TextureUnit.Texture2);
            model.Material.Roughness?.Use(TextureUnit.Texture3);

            Shader.SetInt("material.albedoMap", 0);
            Shader.SetInt("material.normalMap", 1);
            Shader.SetInt("material.metallicMap", 2);
            Shader.SetInt("material.roughnessMap", 3);

            Shader.SetVector3("dirLight.direction", dirLight.Direction);
            Shader.SetColor4("dirLight.ambient", dirLight.Ambient);
            Shader.SetColor4("dirLight.diffuse", dirLight.Diffuse);
            Shader.SetColor4("dirLight.specular", dirLight.Specular);

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
            Shader.SetInt("nbPointLightInScene", lights.Length);

            model.Mesh.Draw();
        }
    }
}