namespace ConsoleRasterizer;

public class ObjImporter
{
    public static Triangle[] Read(string filePath)
    {
        List<string> file = File.ReadAllLines(filePath).ToList();

        List<Vector3> vertices = file
            .Where(x => x.StartsWith("v "))
            .Select(x =>
            {
                var parts = x.Replace('.',',').Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var v = new Vector3(
                    float.Parse(parts[1]),
                    float.Parse(parts[2]),
                    float.Parse(parts[3])
                );

                return v;
            })
            .ToList();

        var triangles = file
            .Where(x => x.StartsWith("f "))
            .Select(x =>
            {
                var parts = x.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var v1Index = int.Parse(parts[1]) - 1;
                var v2Index = int.Parse(parts[2]) - 1;
                var v3Index = int.Parse(parts[3]) - 1;
                return new Triangle(vertices[v1Index], vertices[v2Index], vertices[v3Index]);
            }).ToArray();

        return triangles;
    }
}

