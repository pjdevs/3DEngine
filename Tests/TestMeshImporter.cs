using Xunit;
using gl.Utils;
using gl.Rendering;

namespace gl.Test
{
    public class TestMeshImporter
    {
        [Fact]
        public void TestLoadCube()
        {
            var m = MeshImporter.FromOBJ("../../../Resources/cube.obj");

            Assert.Equal("Cube", m.Name);
            Assert.False(m.Smooth);
            Assert.Equal(8 * 24, m.Vertices.Length);
            Assert.Equal(3 * 2 * 6, m.Indexes.Length); // 6 faces with 2 sets of 3 indices
        }
    }
}