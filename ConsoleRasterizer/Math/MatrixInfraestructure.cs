namespace ConsoleRasterizer;

public partial record struct Matrix4
{
    public static Matrix4 Displace(float dx, float dy, float dz)
    {
        var m = Identity();
        m[0, 3] = dx;
        m[1, 3] = dy;
        m[2, 3] = dz;
        return m;
    }

    public static Matrix4 Scale(float sx, float sy, float sz)
    {
        var m = Identity();
        m[0, 0] = sx;
        m[1, 1] = sy;
        m[2, 2] = sz;
        return m;
    }

    public static Matrix4 Rotate(Axis axis, float angleInDegrees)
    {
        var m = Identity();
        var inRads = ToRadians(angleInDegrees);
        var cos = MathF.Cos(inRads);
        var sin = MathF.Sin(inRads);

        switch (axis)
        {
            case Axis.X:
                m[1, 1] = cos;
                m[1, 2] = -sin;
                m[2, 1] = sin;
                m[2, 2] = cos;
                break;
            case Axis.Y:
                m[0, 0] = cos;
                m[0, 2] = sin;
                m[2, 0] = -sin;
                m[2, 2] = cos;
                break;
            case Axis.Z:
                m[0, 0] = cos;
                m[0, 1] = -sin;
                m[1, 0] = sin;
                m[1, 1] = cos;
                break;
        }

        return m;
    }

    public static Matrix4 Shear(Plane plane, float amount)
    {
        var m = Identity();

        switch (plane)
        {
            case Plane.XY:
                m[0, 2] = amount;
                m[1, 2] = amount;
                break;

            case Plane.XZ:
                m[0, 1] = amount;
                m[2, 1] = amount;
                break;

            case Plane.YZ:
                m[1, 0] = amount;
                m[2, 0] = amount;
                break;
        }

        return m;
    }

    public static float ToRadians(float angleInDegrees) => angleInDegrees * MathF.PI / 180;
}
