using OpenTK.Mathematics;

namespace gl.Rendering.Utils
{
    public static class MeshUtils
    {
        public static Vector3[] ComputeTangents(IEnumerable<Vector3> vertices, IEnumerable<Vector2> UVs, IEnumerable<uint> indices)
        {
            var tangents = new Vector3[vertices.Count()];
            var sharedTangents = new int[tangents.Length];

            for (var i = 0; i < tangents.Length; ++i)
            {
                tangents[i] = Vector3.Zero;
                sharedTangents[i] = 0;
            }

            Vector3? lastTangent = null;

            for (var i = 0; i < indices.Count(); i += 3)
            {
                var i0 = (int)indices.ElementAt(i);
                var i1 = (int)indices.ElementAt(i + 1);
                var i2 = (int)indices.ElementAt(i + 2);

                var pos2 = vertices.ElementAt(i0);
                var pos1 = vertices.ElementAt(i1);
                var pos3 = vertices.ElementAt(i2);

                var uv1 = UVs.ElementAt(i0);
                var uv2 = UVs.ElementAt(i1);
                var uv3 = UVs.ElementAt(i2);

                var edge1 = pos2 - pos1;
                var edge2 = pos3 - pos1;
                var deltaUV1 = uv2 - uv1;
                var deltaUV2 = uv3 - uv1;

                var f = 1.0f / ((deltaUV1.X * deltaUV2.Y) - (deltaUV2.X * deltaUV1.Y));
                if (float.IsInfinity(f))
                    f = 1;

                var tangent = Vector3.Zero;
                tangent.X = (float)Math.Round(f * ((deltaUV2.Y * edge1.X) - (deltaUV1.Y * edge2.X)), 3);
                tangent.Y = (float)Math.Round(f * ((deltaUV2.Y * edge1.Y) - (deltaUV1.Y * edge2.Y)), 3);
                tangent.Z = (float)Math.Round(f * ((deltaUV2.Y * edge1.Z) - (deltaUV1.Y * edge2.Z)), 3);

                // Si on a Tangent = (0,0,0) la normalization donnera Nan dans ce cas on lui donne la valeur de la dernière tangent calculé (ou un valeur aléatoire)
                if (tangent.X == 0 && tangent.Y == 0 && tangent.Z == 0)
                {
                    tangent = lastTangent ?? Vector3.UnitX;
                }

                tangents[i0] += tangent;
                tangents[i1] += tangent;
                tangents[i2] += tangent;

                ++sharedTangents[i0];
                ++sharedTangents[i1];
                ++sharedTangents[i2];

                lastTangent = tangent;
            }

            for (var i = 0; i < tangents.Length; ++i)
            {
                tangents[i] /= sharedTangents[i];
            }

            return tangents;
        }
    }
}