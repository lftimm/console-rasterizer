namespace ConsoleRasterizer;

public enum Axis
{
    X,
    Y,
    Z
}

public enum Plane
{
    XY,
    XZ,
    YZ
}

public static class FloatExtensions
{
    public static bool IsAlmostEqualTo(this float a, float b) => Math.Abs(a - b) < 5e-2f;
}

public record struct Vector4(float X, float Y, float Z, float W)
{
    public Vector4 Homogenize() => new(X / W, Y / W, Z / W, 1f);
    public Vector3 Reduce() => new(X, Y, Z);
    public static Vector4 operator *(Matrix4 m, Vector4 v) => new(
            m[0, 0] * v.X + m[0, 1] * v.Y + m[0, 2] * v.Z + m[0, 3] * v.W,
            m[1, 0] * v.X + m[1, 1] * v.Y + m[1, 2] * v.Z + m[1, 3] * v.W,
            m[2, 0] * v.X + m[2, 1] * v.Y + m[2, 2] * v.Z + m[2, 3] * v.W,
            m[3, 0] * v.X + m[3, 1] * v.Y + m[3, 2] * v.Z + m[3, 3] * v.W
    );
}

public record struct Vector3(float X, float Y, float Z) 
{
    public Vector4 Extend(int w=1) => new Vector4(X,Y,Z,w);
    public float Dot(Vector3 v) => X * v.X + Y * v.Y + Z * v.Z;
    public Vector3 Cross(Vector3 v2) => new(
            Y * v2.Z - Z * v2.Y,
            Z * v2.X - X * v2.Z,
            X * v2.Y - Y * v2.X
    );
    public static float Length(Vector3 v) => (float)Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
    public static Vector3 Normalize(Vector3 v) => v / Length(v);
    public static Vector3 operator *(Vector3 v1, float s) => new(s * v1.X, s * v1.Y, s * v1.Z);
    public static Vector3 operator *(float s, Vector3 v1) => v1 * s;
    public static Vector3 operator /(Vector3 v1, float s) => v1 * (1 / s);
    public static Vector3 operator +(Vector3 v1, Vector3 v2) => new(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
    public static Vector3 operator -(Vector3 v1, Vector3 v2) => new(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
}

public partial record struct Matrix4(float[,] Values)
{
    public readonly float this[int row, int col]
    {
        get => Values[row, col];
        private set => Values[row, col] = value;
    }

    public static Matrix4 New(Vector3 r1, Vector3 r2, Vector3 r3) => new(new float[4, 4]
    {
        { r1.X, r1.Y, r1.Z, 0},
        { r2.X, r2.Y, r2.Z, 0},
        { r3.X, r3.Y, r3.Z, 0},
        {0,     0,     0,   1}
    });

    public static Matrix4 Identity() => new(new float[4, 4]
    {
        {1, 0, 0, 0},
        {0, 1, 0, 0},
        {0, 0, 1, 0},
        {0, 0, 0, 1}
    });

    public Vector3 Transform(Vector3 v) => (this * v.Extend()).Reduce();
    public Triangle Transform(Triangle v) => new Triangle(Transform(v.A), Transform(v.B), Transform(v.C));
    public Triangle[] Transform(Triangle[] v) => v.Select(Transform).ToArray();

    public static Matrix4 operator *(Matrix4 m1, Matrix4 m2)
    {
        var result = new float[4, 4];
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                result[row, col] = 0;
                for (int k = 0; k < 4; k++)
                {
                    result[row, col] += m1[row, k] * m2[k, col];
                }
            }
        }
        return new Matrix4(result);
    }

    public static Matrix4 operator *(Matrix4 m, float s)
    {
        var result = new float[4, 4];
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                result[row, col] = m[row, col] * s;
            }
        }
        return new Matrix4(result);
    }
    
    public static Matrix4 operator *(float s, Matrix4 m) => m * s;

    /// <summary>
    /// Multiplies matrices in the correct order (last to first)
    /// (A, B, C) => A * ( B * C )
    /// </summary>
    public static Matrix4 MultiplyInCorrectOrder(params Matrix4[] matrices)
    {
        if(matrices.Length == 0)
            return Identity();
        
        if(matrices.Length == 1)
            return matrices[0];

        var queue = new Queue<Matrix4>(matrices);
        var result = queue.Dequeue();

        while( queue.Count > 0 )
            result *= queue.Dequeue();

        return result;
    }
}
