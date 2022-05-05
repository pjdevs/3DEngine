using System.Globalization;

namespace gl.Utils
{
    public class MeshData
    {
        public readonly string Name;
        public readonly float[] Vertices;
        public readonly uint[] Indexes;
        public readonly bool Smooth;

        public MeshData(string name, float[] vertices, uint[] indexes, bool smooth)
        {
            Name = name;
            Vertices = vertices;
            Indexes = indexes;
            Smooth = smooth;
        }
    }

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

        public static MeshData FromOBJ(string path)
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
                                vertexBuffer.AddRange(vertices.Skip((int)triplet.Item1 * 3).Take(3));
                                vertexBuffer.AddRange(uvs.Skip((int)triplet.Item2 * 2).Take(2));
                                vertexBuffer.AddRange(normals.Skip((int)triplet.Item3 * 3).Take(3));

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

            return new MeshData(name, vertexBuffer.ToArray(), indices.ToArray(), smooth);
        }
    }
}
