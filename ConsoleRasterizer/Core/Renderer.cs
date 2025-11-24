namespace ConsoleRasterizer;
public class Renderer
{
    private const float Step = 0.2f;
    private int Width { get; }
    private int Height { get; }
    private float AspectRatio { get; } = 2f;
    public Matrix4 ProjectionMatrix { get; private set; }
    public float NearPlane { get; }
    public float FarPlane { get; }
    public float FieldOfView { get; }
    public float TopPlane { get; }
    public float RightPlane { get; }
    public float TanHalfFov { get; }
    private Matrix4 _viewMatrix;
    public Matrix4 ViewMatrix
    {
        get { return _viewMatrix; }
        set { _viewMatrix = value; CalculateProjection(_viewMatrix);  }
    }

    private Vector3 _eye = new(0f, 0f, -1f);
    private Vector3 _gaze = new(0f, 0f, -1f);
    private Vector3 _up = new(0f, 1f, 0f);

    public Renderer(int width, int height, KeyboardEventHandler keyboardEventHandler)
    {
        keyboardEventHandler.OnKeyPress += OnKeyPressed;
        Width = width;
        Height = height;
        ViewMatrix = new View(_eye,_gaze, _up).Transform;


        FieldOfView = 90f;
        TanHalfFov = MathF.Tan(Matrix4.ToRadians(FieldOfView / 2));

        FarPlane = -1f;
        NearPlane = 1f;
        TopPlane = NearPlane * TanHalfFov;
        RightPlane = Width / (Height*2) * TopPlane ;
    }

    private void CalculateProjection(Matrix4 viewMatrix)
    {
        var orthogonalMatrix = CreateOrthogonalProjectionMatrix();
        var aspectRatioFix = CreateAspectRatioProjectionMatrix();
        var perspectiveMatrix = CreatePerspectiveProjectionMatrix();

        ProjectionMatrix =
            Matrix4.MultiplyInCorrectOrder(aspectRatioFix, orthogonalMatrix, perspectiveMatrix, viewMatrix);
    }


    private void OnKeyPressed(object? sender, KeyboardEventArgs e)
    {
        Vector3 forward = Vector3.Normalize(_gaze);
        Vector3 right = Vector3.Normalize(forward.Cross(_up));

        switch (e.Key)
        {
            case ConsoleKey.W:
                _eye -= forward * Step;
                break;
            case ConsoleKey.S:
                _eye += forward * Step;
                break;
            case ConsoleKey.A:
                _eye -= right * Step;
                break;
            case ConsoleKey.D:
                _eye += right * Step;
                break;
            case ConsoleKey.Q: 
                _gaze = Matrix4.Rotate(Axis.Y, -Step * 10).Transform(_gaze);
                break;
            case ConsoleKey.E: 
                _gaze = Matrix4.Rotate(Axis.Y, +Step * 10).Transform(_gaze);
                break;
            case ConsoleKey.Spacebar:
                _eye += _up * Step;
                break;
            case ConsoleKey.C:
                _eye -= _up * Step;
                break;
        }

        ViewMatrix = new View(_eye, _gaze, _up).Transform;

    }

    private Matrix4 CreatePerspectiveProjectionMatrix()
    {
        var n = NearPlane;
        var f = FarPlane;

        var m = new Matrix4(new float[4, 4]
        {
            {n,0f,0f,0f},
            {0f,n,0f,0f},
            {0f,0f,n+f,-f*n},
            {0f,0f,1f,0f}
        });

        return m;
    }

    private Matrix4 CreateAspectRatioProjectionMatrix()
    {
        var inverseAspectRatio = 1 / AspectRatio;
        var aspectRatioFix = Matrix4.Scale(inverseAspectRatio, 1, 1) * Matrix4.Displace(Width * inverseAspectRatio, 0, 0);
        return aspectRatioFix;
    }

    public void Render(FrameBuffer buffer, Triangle triangle)
    {
        if(buffer.Width != Width || buffer.Height != Height)
            throw new ArgumentException("FrameBuffer size does not match Renderer size.");

        var stepX = .5f;
        var stepY = .5f;

        // Vertex shader 
        var a4 = (ProjectionMatrix * triangle.A.Extend());
        var b4 = (ProjectionMatrix * triangle.B.Extend());
        var c4 = (ProjectionMatrix * triangle.C.Extend());

        if (a4.W <= 0.01f || b4.W <= 0.01f || c4.W <= 0.01f)
            return;

        var a = a4.Homogenize().Reduce();
        var b = b4.Homogenize().Reduce();
        var c = c4.Homogenize().Reduce();

        // Tesselation
        var triangleCoords = 
            GenerateTriangleCoordinates(a, b, c, stepX, stepY);

        // Fragment shader
        var brightnessValues = new List<int>();
        for (int i = 0; i < triangleCoords.Count; i += 3)
        {
            var z = triangleCoords[i + 2];
            var brightness = (int)(Math.Clamp(z, -1, 1) * 10 + 10);
            brightnessValues.Add(brightness);
        }

        /// Wrap
        for(int i = 0; i < triangleCoords.Count; i += 3)
        {
            var x = (int)triangleCoords[i];
            var y = (int)triangleCoords[i + 1];
            var brightness = brightnessValues[i / 3];
            var pixel = new Pixel(brightness);
            buffer.SetPixel(x, y, pixel); 
        }
        
    }

    private Matrix4 CreateOrthogonalProjectionMatrix()
    {
        var r = RightPlane;
        var l = -r;
        var t = TopPlane;
        var b = -t;
        var n = NearPlane;
        var f = FarPlane;

        var scaleX = Width / 2;
        var scaleY = Height / 2;
        var m1 = Matrix4.Displace(scaleX, scaleY, 0) *
                 Matrix4.Scale(scaleX, -scaleY, 1.0f);
        var m2 = Matrix4.Scale(2/(r-l), 2/(t-b), 2/(n-f));
        var m3 = Matrix4.Displace(-(l+r)/2, -(b+t)/2, -(f+n)/2);

        return Matrix4.MultiplyInCorrectOrder(m1, m2, m3); 
    }

    private static List<float> GenerateTriangleCoordinates(Vector3 a, Vector3 b, Vector3 c, float stepX, float stepY)
    {
        var xmin = (float)Math.Min(Math.Min(a.X, b.X), c.X);
        var xmax = (float)Math.Max(Math.Max(a.X, b.X), c.X);
        var ymin = (float)Math.Min(Math.Min(a.Y, b.Y), c.Y);
        var ymax = (float)Math.Max(Math.Max(a.Y, b.Y), c.Y);

        float getGamma(float x, float y)
        {
            var numerator = (a.Y - b.Y) * x + (b.X - a.X) * y + a.X * b.Y - b.X * a.Y;
            var denominator = (a.Y - b.Y) * c.X + (b.X - a.X) * c.Y + a.X * b.Y - b.X * a.Y;
            return numerator / denominator;
        }

        float getBeta(float x, float y)
        {
            var numerator = (a.Y - c.Y) * x + (c.X - a.X) * y + a.X * c.Y - c.X * a.Y;
            var denominator = (a.Y - c.Y) * b.X + (c.X - a.X) * b.Y + a.X * c.Y - c.X * a.Y;
            return numerator / denominator;
        }

        List<float> coordinates = new List<float>();
        for (float x = xmin; x <= xmax; x += stepX)
        {
            for(float y = ymin; y <= ymax; y += stepY)
            {
                var beta = getBeta(x, y);
                var gamma = getGamma(x, y);
                var alpha = 1 - beta - gamma;

                if(!(beta.IsAlmostEqualTo(0) || gamma.IsAlmostEqualTo(0) || alpha.IsAlmostEqualTo(0)))
                    continue;

                if (beta >= 0 && gamma >= 0 && (beta + gamma) <= 1)
                {
                    var z = alpha * a.Z + beta * b.Z + gamma * c.Z;
                    coordinates.Add(x);
                    coordinates.Add(y);
                    coordinates.Add(z);
                }
            }
        }

        return coordinates;
    }
}
