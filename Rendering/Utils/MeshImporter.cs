using System.Globalization;
using OpenTK.Mathematics;

namespace gl.Rendering.Utils
{
    public static class MeshImporter
    {
        private static string ObjectName(string[]? args)
        {
            if (args?.Length != 1)
            {
                throw new Exception("o command must have one argument");
            }

            return args[0];
        }

        private static bool Smooth(string[]? args)
        {
            if (args?.Length != 1)
            {
                throw new Exception("s command must have one argument");
            }

            return args[0] switch
            {
                "1" or "on" => true,
                "0" or "off" => false,

                _ => throw new Exception($"Unknown smooth value {args[0]}")
            };
        }

        private static IEnumerable<T> ReadNumbers<T>(string[]? args, int count, Func<string, IFormatProvider, T> coverter)
        {
            if (args?.Length != count)
            {
                throw new Exception($"this v commands must have {count} argument");
            }

            foreach (var arg in args)
            {
                yield return coverter.Invoke(arg, CultureInfo.InvariantCulture);
            }
        }

        private const int STRIDE = 3 + 2 + 3 + 3; // pos + uv + normal + tangent

        private static Vector3 GetPos(List<float> vertexBuffer, int index)
        {
            return new Vector3(
                vertexBuffer[index * STRIDE],
                vertexBuffer[index * STRIDE + 1],
                vertexBuffer[index * STRIDE + 2]
            );
        }

        private static Vector2 GetUV(List<float> vertexBuffer, int index)
        {
            return new Vector2(
                vertexBuffer[index * STRIDE + 3],
                vertexBuffer[index * STRIDE + 4]
            );
        }

        private static void SetTangent(List<float> vertexBuffer, int index, Vector3 tangent)
        {
            vertexBuffer[index * STRIDE + STRIDE - 3] = tangent.X;
            vertexBuffer[index * STRIDE + STRIDE - 2] = tangent.Y;
            vertexBuffer[index * STRIDE + STRIDE - 1] = tangent.Z;
        }

        private static void ComputeTangents(List<float> vertexBuffer, List<uint> indices)
        {
            for (var i = 0; i < indices.Count; i += 3)
            {
                var i0 = (int)indices[i];
                var i1 = (int)indices[i + 1];
                var i2 = (int)indices[i + 2];

                var pos0 = GetPos(vertexBuffer, i0);
                var pos1 = GetPos(vertexBuffer, i1);
                var pos2 = GetPos(vertexBuffer, i2);

                var UV0 = GetUV(vertexBuffer, i0);
                var UV1 = GetUV(vertexBuffer, i1);
                var UV2 = GetUV(vertexBuffer, i2);

                var deltaPos1 = pos1 - pos0;
                var deltaPos2 = pos2 - pos0;

                var deltaUV1 = UV1 - UV0;
                var deltaUV2 = UV2 - UV0;

                var tangent = (deltaPos1 * deltaUV2.Y - deltaPos2 * deltaUV1.Y) / (deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X);

                SetTangent(vertexBuffer, i0, tangent);
                SetTangent(vertexBuffer, i1, tangent);
                SetTangent(vertexBuffer, i2, tangent);
            }
        }

        public static Mesh FromOBJ(string path, bool computeTangents = false)
        {
            var reader = new StreamReader(path);
            var line = reader.ReadLine();

            var vertices = new List<float>();
            var uvs = new List<float>();
            var normals = new List<float>();
            var indices = new List<uint>();
            var name = string.Empty;
            var smooth = false;

            var nextIndex = 0u;
            var tripletsIndexes = new Dictionary<(uint, uint, uint), uint>();
            var vertexBuffer = new List<float>();

            while (line != null)
            {
                var tokens = line.Split(' ', StringSplitOptions.TrimEntries);
                var command = tokens[0];
                var args = tokens[1..];

                switch (command)
                {
                    case "#":
                        break;
                    case "o":
                        name = ObjectName(args);
                        break;
                    case "s":
                        smooth = Smooth(args);
                        break;
                    case "v":
                        vertices.AddRange(ReadNumbers(args, 3, float.Parse));
                        break;
                    case "vn":
                        normals.AddRange(ReadNumbers(args, 3, float.Parse));
                        break;
                    case "vt":
                        uvs.AddRange(ReadNumbers(args, 2, float.Parse));
                        break;
                    case "f":
                        foreach (var arg in args)
                        {
                            var tripletArray = ReadNumbers(arg.Split('/'), 3, uint.Parse).ToArray();
                            var triplet = (tripletArray[0] - 1, tripletArray[1] - 1, tripletArray[2] - 1);

                            if (!tripletsIndexes.TryGetValue(triplet, out var index))
                            {
                                var vertex = vertices.Skip((int)triplet.Item1 * 3).Take(3);
                                var uv = uvs.Skip((int)triplet.Item2 * 2).Take(2);
                                var normal = normals.Skip((int)triplet.Item3 * 3).Take(3);

                                vertexBuffer.AddRange(vertex);
                                vertexBuffer.AddRange(uv);
                                vertexBuffer.AddRange(normals);

                                if (computeTangents)
                                {
                                    vertexBuffer.Add(0f);
                                    vertexBuffer.Add(0f);
                                    vertexBuffer.Add(0f);
                                }

                                indices.Add(nextIndex);
                                tripletsIndexes.Add(triplet, nextIndex);
                                nextIndex++;
                            }
                            else
                            {
                                indices.Add(index);
                            }
                        }
                        break;

                    default:
                        throw new Exception($"Unsupported command {command}");
                };

                line = reader.ReadLine();
            }

            if (computeTangents)
                ComputeTangents(vertexBuffer, indices);

            return new Mesh(
                vertexBuffer.ToArray(),
                indices.ToArray(),
                MeshFlags.Vertices
              | MeshFlags.UVs
              | MeshFlags.Normals
              | (computeTangents ? MeshFlags.Tangents : MeshFlags.Vertices)
            );
        }
    }
}
