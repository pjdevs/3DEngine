using OpenTK.Mathematics;

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
                 halfWidth, 0.0f,  halfHeight, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f, // top right
                 halfWidth, 0.0f, -halfHeight, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f, // bottom right
                -halfWidth, 0.0f, -halfHeight, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f, // bottom left
                -halfWidth, 0.0f,  halfHeight, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f, // top left
            }, new uint[]
            {
                0, 1, 3, // The first triangle will be the bottom-right half of the triangle
                1, 2, 3  // Then the second will be the top-right half of the triangle
            }, MeshFlags.All);
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

        public static Mesh BuildSphere(float radius, int sectorCount, int stackCount = -1)
        {
            stackCount = stackCount == -1 ? sectorCount : stackCount;

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> texCoords = new List<Vector2>();

            float x, y, z, xy;                              // vertex position
            float nx, ny, nz, lengthInv = 1.0f / radius;    // vertex normal
            float s, t;                                     // vertex texCoord

            float sectorStep = (float)(2 * Math.PI / sectorCount);
            float stackStep = (float)(Math.PI / stackCount);
            float sectorAngle, stackAngle;

            for (int i = 0; i <= stackCount; ++i)
            {
                stackAngle = (float)Math.PI / 2 - i * stackStep;        // starting from pi/2 to -pi/2
                xy = radius * (float)Math.Cos(stackAngle);             // r * cos(u)
                z = radius * (float)Math.Sin(stackAngle);              // r * sin(u)

                // add (sectorCount+1) vertices per stack
                // the first and last vertices have same position and normal, but different tex coords
                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;           // starting from 0 to 2pi

                    // vertex position (x, y, z)
                    x = xy * (float)Math.Cos(sectorAngle);             // r * cos(u) * cos(v)
                    y = xy * (float)Math.Sin(sectorAngle);             // r * cos(u) * sin(v)
                    vertices.Add(new Vector3(x, y, z));

                    // normalized vertex normal (nx, ny, nz)
                    nx = x * lengthInv;
                    ny = y * lengthInv;
                    nz = z * lengthInv;
                    normals.Add(new Vector3(nx, ny, nz));

                    // vertex tex coord (s, t) range between [0, 1]
                    s = (float)j / sectorCount;
                    t = stackCount - ((float)i / stackCount); // Ici on inverse cas on flip les texture en Y quand on les importe
                    texCoords.Add(new Vector2(s, t));
                }
            }

            // generate CCW index list of sphere triangles
            // k1--k1+1
            // |  / |
            // | /  |
            // k2--k2+1
            List<uint> indices = new List<uint>();
            int k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = i * (sectorCount + 1);     // beginning of current stack
                k2 = k1 + sectorCount + 1;      // beginning of next stack

                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    // 2 triangles per sector excluding first and last stacks
                    // k1 => k2 => k1+1
                    if (i != 0)
                    {
                        indices.Add((uint)k1);
                        indices.Add((uint)k2);
                        indices.Add((uint)k1 + 1);
                    }

                    // k1+1 => k2 => k2+1
                    if (i != (stackCount - 1))
                    {
                        indices.Add((uint)k1 + 1);
                        indices.Add((uint)k2);
                        indices.Add((uint)k2 + 1);
                    }
                }
            }

            var vertexBuffer = new float[(vertices.Length + normals.Length) * 3 + texCoords.Length * 2];

            // TODO: for loop to fill vertexBuffer

            Mesh = new Mesh(vertexBuffer, indices);
        }
    }
}