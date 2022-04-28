namespace gl.Rendering
{
    public class Model
    {
        public Transform Transform;
        public Material Material;
        public Mesh Mesh { get; private set; }

        public Model(Mesh mesh)
        {
            Transform = new Transform();
            Material = new Material();
            Mesh = mesh;
        }
    }
}