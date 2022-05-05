namespace gl.Rendering
{
    public static class MeshBuilder
    {
        public static Mesh BuildPlane(float width, float height)
        {
            var halfWidth = width / 2;
            var halfHeight = height / 2;

            return new Mesh(new float[]
            {
                 halfWidth, 0.0f,  halfHeight, 1.0f, 1.0f, // top right
                 halfWidth, 0.0f, -halfHeight, 1.0f, 0.0f, // bottom right
                -halfWidth, 0.0f, -halfHeight, 0.0f, 0.0f, // bottom left
                -halfWidth, 0.0f,  halfHeight, 0.0f, 1.0f, // top left
            }, new uint[]
            {
                0, 1, 3, // The first triangle will be the bottom-right half of the triangle
                1, 2, 3  // Then the second will be the top-right half of the triangle
            });
        }

        public static Mesh BuildCube(float size)
        {
            return new Mesh(new float[]
            {
                // left down front vertex (for the 3 sides)
                -size, -size, -size, 0.0f, 0.0f, // front face
                -size, -size, -size, 1.0f, 0.0f, // left face
                -size, -size, -size, 0.0f, 1.0f, // bottom face

                // right down front
                 size, -size, -size, 1.0f, 0.0f, // front face
                 size, -size, -size, 0.0f, 0.0f, // right face
                 size, -size, -size, 1.0f, 1.0f, // bottom face

                // right down back
                 size, -size,  size, 0.0f, 0.0f, // back face
                 size, -size,  size, 1.0f, 0.0f, // right face
                 size, -size,  size, 1.0f, 0.0f, // bottom face 

                // left down back
                -size, -size,  size, 1.0f, 0.0f, // back face
                -size, -size,  size, 0.0f, 0.0f, // left face
                -size, -size,  size, 0.0f, 0.0f, // bottom face 

                // left up front
                -size,  size, -size, 0.0f, 1.0f, // front face
                -size,  size, -size, 1.0f, 1.0f, // left face
                -size,  size, -size, 0.0f, 0.0f, // top face 

                // right up front
                 size,  size, -size, 1.0f, 1.0f, // front face
                 size,  size, -size, 0.0f, 1.0f, // right face
                 size,  size, -size, 1.0f, 0.0f, // top face 

                // right up back
                 size,  size,  size, 0.0f, 1.0f, // back face
                 size,  size,  size, 1.0f, 1.0f, // right face
                 size,  size,  size, 1.0f, 1.0f, // top face 

                // left up back
                -size,  size,  size, 1.0f, 1.0f, // back face
                -size,  size,  size, 0.0f, 1.0f, // left face
                -size,  size,  size, 0.0f, 1.0f, // top face 
            },
            new uint[]
            {
                0,  3,  12, // front
                3,  12, 15,
                6,  9,  18, // back
                9,  18, 21,
                5,  2,  8,  // bottom
                2,  8,  11,
                17, 14, 20, // top
                14, 20, 23,
                1,  10, 13, // left
                10, 13, 22,
                7,  4,  19, // right
                4,  19, 16
            });
        }
    }
}