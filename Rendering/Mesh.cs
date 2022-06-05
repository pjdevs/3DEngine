using OpenTK.Graphics.OpenGL4;

namespace gl.Rendering
{
    public enum MeshFlags
    {
        Vertices = 1,
        UVs = 2,
        Normals = 4,
        Tangents = 8,
        All = Vertices | UVs | Normals | Tangents
    }

    public class Mesh : IDisposable
    {
        private readonly float[] _vertices;
        private readonly uint[] _indices;
        private readonly int _vertexBufferObject;
        private readonly int _vertexArrayObject;
        private readonly int _elementBufferObject;

        public bool Smooth { get; set; }

        public Mesh(float[] vertices, uint[] indices, MeshFlags flags)
        {
            var includeVertices = (flags & MeshFlags.Vertices) == MeshFlags.Vertices;
            var includeUVs = (flags & MeshFlags.UVs) == MeshFlags.UVs;
            var includeNormals = (flags & MeshFlags.Normals) == MeshFlags.Normals;
            var includeTangents = (flags & MeshFlags.Tangents) == MeshFlags.Tangents;

            if (!includeVertices)
                throw new Exception("Mesh vertices must at least contains vertex coordinates");

            var currentIndex = 0;
            var currentOffset = 0;
            var stride = (3 + Convert.ToInt32(includeUVs) * 2 + Convert.ToInt32(includeNormals) * 3 + Convert.ToInt32(includeTangents) * 3) * sizeof(float);

            _vertices = vertices;
            _indices = indices;

            _vertexBufferObject = GL.GenBuffer();
            _vertexArrayObject = GL.GenVertexArray();
            _elementBufferObject = GL.GenBuffer();

            Smooth = false;

            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(currentIndex);
            GL.VertexAttribPointer(currentIndex, 3, VertexAttribPointerType.Float, false, stride, 0);
            currentOffset += 3;
            currentIndex++;

            if (includeUVs)
            {
                GL.EnableVertexAttribArray(currentIndex);
                GL.VertexAttribPointer(currentIndex, 2, VertexAttribPointerType.Float, false, stride, currentOffset * sizeof(float));
                currentOffset += 2;
                currentIndex++;
            }
            if (includeNormals)
            {
                GL.EnableVertexAttribArray(currentIndex);
                GL.VertexAttribPointer(currentIndex, 3, VertexAttribPointerType.Float, false, stride, currentOffset * sizeof(float));
                currentOffset += 3;
                currentIndex++;
            }
            if (includeTangents)
            {
                GL.VertexAttribPointer(currentIndex, 3, VertexAttribPointerType.Float, false, stride, currentOffset * sizeof(float));
                GL.EnableVertexAttribArray(currentIndex);
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public Mesh(float[] vertices, uint[] indices)
            : this(vertices, indices, MeshFlags.Vertices | MeshFlags.UVs)
        {
        }

        public void Draw()
        {
            // if (Smooth)
            // {
            //     // TODO: Shade model smooth
            // }
            // else
            // {
            //     // TODO: Shade model flat
            // }

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(_vertexArrayObject);
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_elementBufferObject);
            GC.SuppressFinalize(this);
        }
    }
}