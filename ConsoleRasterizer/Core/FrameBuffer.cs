namespace ConsoleRasterizer;

public class FrameBuffer
{
    private readonly Pixel[] _buffer;
    public int Width { get; }
    public int Height { get; } 


    public FrameBuffer(int width, int height)
    {
        Width = width;
        Height = height;
        _buffer = new Pixel[Height*Width];
    }

    public Pixel GetPixel(int x, int y) => _buffer[x+y*Width];
    public int GetBrightnessIn(int x, int y) => GetPixel(x,y).Brightness;
    public void SetPixel(int x, int y, Pixel pix)
    {
        if(x < 0 || x >= Width || y < 0 || y >= Height)
            return;
        _buffer[x + y * Width] = pix;
    }
    public void Clear(Brightness brightness)
    {
        for(int i = 0; i < _buffer.Length; i++)
            _buffer[i] = Pixel.WithBrightness(brightness); 
    }
    public static void Copy(FrameBuffer source, FrameBuffer destination)
    {
        if (source.Width != destination.Width || source.Height != destination.Height)
            throw new ArgumentException("FrameBuffer sizes do not match.");

        Array.Copy(source._buffer, destination._buffer, source._buffer.Length);
    }
}
